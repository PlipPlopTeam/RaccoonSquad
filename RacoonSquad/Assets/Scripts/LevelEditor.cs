using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XInputDotNetPure;
using System.Xml;

public class LevelEditor : MonoBehaviour
{
    public GameObject saveMenu;
    public GameObject saveProgressionScreen;
    public TextMeshProUGUI saveProgression;
    public Slider saveSlider;
    public GameObject container;
    public TextMeshProUGUI title;
    public TextMeshProUGUI levelName;
    public float objectsMovementSpeed = 5f;
    public float objectsRotationSpeed = 30f;
    public KeyCode snapKey = KeyCode.LeftShift;
    public KeyCode menuKey = KeyCode.Space;

    int currentCursor = 0;
    Dictionary<Image, GameObject> entries = new Dictionary<Image, GameObject>();
    bool canMoveInMenus = true;
    bool isMenuing = false;
    bool justPressedA = false;
    bool justToggled = false;
    GameObject ghost;
    Vector2 previousMousePosition;
    Vector2 previousLeftStick;
    float savingAdvancement = 0f;

    public class SavedProp{
        public GameObject prefab;
        public Vector3 position;
        public Vector3 euler;
    }

    private void Start()
    {
        /*
        SoundPlayer.StopEverySound();
        SoundPlayer.Play("msc_editor");
        */
        Instantiate(Library.instance.editorSaveLevelCube, new Vector3(), Quaternion.identity);
        StartCoroutine(CreatePreviews());
    }

    private void Update()
    {
        var state = GamePad.GetState(GameManager.instance.GetPlayers()[0].index);
        var pc = FindObjectOfType<PlayerController>();

        UpdateVisuals();
        CheckToggleInput(state);
        pc.Paralyze();

        if (isMenuing)
        {
            CheckInputs(state);
            UpdateSelection();
        }
        else if (ghost != null)
        {
            CheckPlacementInputs(state);
        }
        else
        {
            pc.Free();
        }
    }

    public void ShowSaveMenu()
    {
        saveMenu.SetActive(true);
        FindObjectOfType<PlayerController>().Paralyze();
    }

    void SelectItem(GameObject item)
    {
        for (int i = 0; i < container.transform.childCount; i++) {
            var t = container.transform.GetChild(i);
            if (t.gameObject == item) {
                currentCursor = i;
                return;
            }
        }
    }

    void CheckPlacementInputs(GamePadState state)
    {
        var leftThumbstickVector = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        var rightThumbstickVector = new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);

        if ((new Vector2(Input.mousePosition.x, Input.mousePosition.y) - previousMousePosition).magnitude > 0) {
            SetGhostPositionFromMouse(Input.mousePosition);
        }
        else {
            UpdateGhostPosition(leftThumbstickVector * objectsMovementSpeed * Time.deltaTime, state.Buttons.RightShoulder == ButtonState.Pressed || Input.GetKey(snapKey));
        }

        UpdateGhostRotation(
            new Vector2(Input.mouseScrollDelta.y, 0f)* 5f + rightThumbstickVector * objectsRotationSpeed * Time.deltaTime, 
            state.Buttons.RightShoulder == ButtonState.Pressed || Input.GetKey(snapKey));

        if ((state.Buttons.B == ButtonState.Pressed) || Input.GetMouseButtonDown(1))
        {// Cancel
            DestroyGhost();
        }
        if ((state.Buttons.A == ButtonState.Pressed && !justPressedA) || Input.GetMouseButtonDown(0))
        {// Place
            Unghost();
        }
        if (state.Buttons.A == ButtonState.Released)
        {
            justPressedA = false;
        }

        previousMousePosition = Input.mousePosition;
    }

    void SetGhostPositionFromMouse(Vector2 mousePos)
    {
        var ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        UpdateGhostPosition(new Vector2(hit.point.x, hit.point.z) - new Vector2(ghost.transform.position.x, ghost.transform.position.z), Input.GetKey(snapKey));
    }

    void DestroyGhost()
    {
        Destroy(ghost);
        ghost = null;
    }

    void Unghost()
    {
        var prop = ghost.transform.GetChild(0).gameObject;

        var renderer = prop.GetComponentInChildren<Renderer>();
        renderer.material.SetFloat("_Opacity", 1f);

        prop.GetComponent<Collider>().enabled = true;
        prop.GetComponent<Rigidbody>().isKinematic = false;
        prop.transform.parent = null;
        DestroyGhost();
    }
    void UpdateGhostRotation(Vector2 rotation, bool snap=false)
    {
        var prop = ghost.transform.GetChild(0).gameObject;
        prop.transform.eulerAngles += (new Vector3(rotation.y, rotation.x));

        if (snap)
        {
            var unit = 45f;
            prop.transform.eulerAngles = new Vector3(
                Mathf.Round(prop.transform.eulerAngles.x / unit) * unit,
                Mathf.Round(prop.transform.eulerAngles.y / unit) * unit,
                Mathf.Round(prop.transform.eulerAngles.z / unit) * unit
            );
        }
    }
    void UpdateGhostPosition(Vector2 displacement, bool snap=false)
    {
        ghost.transform.position += new Vector3(displacement.x, 0f, displacement.y);
        RaycastHit hit;
        Physics.Raycast(ghost.transform.position, Vector3.down, out hit);

        var prop = ghost.transform.GetChild(0).gameObject;
        var offset = prop.GetComponent<Prop>().collider.bounds.extents.y / 2f + 0.5f;
        prop.transform.position = new Vector3(ghost.transform.transform.position.x, hit.point.y + offset, ghost.transform.position.z);

        if (snap)
        {
            var unit = 0.5f;
            prop.transform.position = new Vector3(
                Mathf.Round(prop.transform.position.x / unit) * unit,
                prop.transform.position.y,
                Mathf.Round(prop.transform.position.z / unit) * unit
            );
        }
    }

    void CheckToggleInput(GamePadState state)
    {
        if ((state.Buttons.Start == ButtonState.Pressed && !justToggled) || Input.GetKeyDown(menuKey))
        {
            DestroyGhost();
            isMenuing = !isMenuing;
            justToggled = true;
        }
        if (state.Buttons.Start == ButtonState.Released || Input.GetKeyUp(menuKey))
        {
            justToggled = false;
        }
    }

    GameObject MakeGhost(GameObject propInstance)
    {
        ghost = new GameObject("_PLACEMENT_GHOST");
        ghost.transform.position = propInstance.transform.position + new Vector3(0f, 10f, 0f);
        propInstance.transform.parent = ghost.transform;
        propInstance.transform.localPosition = new Vector3();

        
        var renderer = propInstance.GetComponentInChildren<Renderer>();
        renderer.material = Instantiate(renderer.material);
        renderer.material.SetFloat("_Opacity", 0.5f);



        propInstance.GetComponent<Collider>().enabled = false;
        propInstance.GetComponent<Rigidbody>().isKinematic = true;
        ghost.AddComponent<BoxCollider>();

        return ghost;
    }

    void UpdateVisuals()
    {
        container.SetActive(isMenuing);
        title.transform.parent.gameObject.SetActive(isMenuing);
    }

    void UpdateSelection()
    {
        int i = 0;
        var containerImg = container.GetComponent<Image>();
        foreach (var img in entries.Keys)
        {
            if (i == currentCursor)
            {
                img.color = Color.white;
                title.text = entries[img].name;
            }
            else
            {
                img.color = containerImg.color;
            }
            i++;
        }
    }

    void CheckInputs(GamePadState state)
    {

        var rowsAndColumns = CountRowsAndColumns();
        var rows = rowsAndColumns.Item1;
        var columns = rowsAndColumns.Item2;
        var thumbstickVector = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);

        if (thumbstickVector.magnitude <= 0) canMoveInMenus = true;
        else if (canMoveInMenus)
        {
            Vector2Int roundedTS =
                new Vector2Int(
                    Mathf.RoundToInt(Mathf.Clamp(thumbstickVector.x * 6f, -1f, 1f)),
                    Mathf.RoundToInt(Mathf.Clamp(thumbstickVector.y * 6f, -1f, 1f))
               )
            ;

            currentCursor = Mathf.Clamp(
                currentCursor - roundedTS.y * rows + roundedTS.x,
                0, container.transform.childCount - 1);

            canMoveInMenus = false;
        }

        if (state.Buttons.A == ButtonState.Pressed || Input.GetMouseButton(0))
        {
            var prop = Instantiate(
                entries[entries.Keys.ToList()[currentCursor]]
            );
            var grab = prop.GetComponent<Grabbable>();
            if (!grab) grab = prop.AddComponent<Grabbable>();

            ghost = MakeGhost(prop);
            isMenuing = false;
            justPressedA = true;
        }
    }

    System.Tuple<int, int> CountRowsAndColumns()
    {
        var grid = container.GetComponent<GridLayoutGroup>();
        if (grid.transform.childCount <= 0) return new System.Tuple<int, int>(0, 0) ;

        var rows = 1;
        var columns = 1;
        var firstChildPos = grid.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
        bool stopCountingRow = false;

        //Loop through the rest of the child object
        for (int i = 1; i < grid.transform.childCount; i++)
        {
            //Get the next child
            var currentChildObj = grid.transform.
           GetChild(i).GetComponent<RectTransform>();

            var currentChildPos = currentChildObj.anchoredPosition;

            //if first child.x == otherchild.x, it is a column, ele it's a row
            if (firstChildPos.x == currentChildPos.x)
            {
                columns++;
                stopCountingRow = true;
            }
            else
            {
                if (!stopCountingRow)
                    rows++;
            }
        }

        return new System.Tuple<int, int>(rows, columns);

    }

    IEnumerator CreatePreviews()
    {
        foreach(var prop in Library.instance.props)
        {
            if (!prop.GetComponent<Prop>()) continue;

            var g = new GameObject();
            g.transform.parent = container.transform;

            var element = g.AddComponent<LevelEditorMenuElement>();
            element.editor = this;
            element.onHovered += delegate {
                SelectItem(g);
            };

            var img = g.AddComponent<Image>();
            var containerImg = container.GetComponent<Image>();
            img.sprite = containerImg.sprite;
            img.color = containerImg.color;
            img.type = Image.Type.Sliced;
            entries.Add(img, prop);
            var msk = g.AddComponent<Mask>();

            var preview = new GameObject();
            preview.transform.parent = g.transform;
            var rect = preview.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);


            var bounds = prop.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.extents;
            var avgBounds = (bounds.x + bounds.y + bounds.z) / 0.5f;
            var sizeRatio = Mathf.Clamp(1f / avgBounds, 0.1f, 1f);
            var displayer = DisplayerManager.SetRotationFeed(
                null,
                preview.AddComponent<RawImage>(),
                512,
                sizeRatio,
                Random.value,
                5f*sizeRatio
            );
            displayer.name = prop.name;

            var tgt = displayer.GetModel();
            try
            {
                tgt.AddComponent<MeshRenderer>().material = prop.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                tgt.AddComponent<MeshFilter>().mesh = prop.GetComponentInChildren<MeshFilter>().sharedMesh;

            }
            catch
            {
                Debug.Log("Could not load displayer for prop " + prop.name);
            }
            yield return false;
        }

        yield return true;
    }

    public void Save()
    {
        var name = levelName.text;
        saveMenu.SetActive(false);
        saveProgressionScreen.SetActive(true);
        StartCoroutine(SaveLevel("level_"+name));
    }

    IEnumerator SaveLevel(string name)
    {
        XmlDocument xDoc = new XmlDocument();
        XmlElement xDocElement = (XmlElement)xDoc.AppendChild(xDoc.CreateElement("Level"));
        XmlElement xProps = (XmlElement)xDocElement.AppendChild(xDoc.CreateElement("Props"));

        var props = FindObjectsOfType<Prop>();
        for (int i = 0; i < props.Length; i++) {
            var prop = props[i];

            // checking it's part of the library
            var skip = true;
            foreach (var lProp in Library.instance.props) {
                var lPropC = lProp.GetComponent<Prop>();
                if (lPropC == null) continue;
                if (lPropC.id == prop.id) {
                    skip = false;
                    break;
                }
                yield return null;
            }
            if (skip) continue;

            // updating progress bar
            savingAdvancement = ((float)i) / props.Length;
            saveProgression.text = Mathf.Round(savingAdvancement * 100f) + "%";
            saveSlider.value = savingAdvancement;

            // actual saving
            var node = xProps.AppendChild(xDoc.CreateElement("Prop"));
            ((XmlElement)node).SetAttribute("id", prop.id.ToString());
            var position = node.AppendChild(xDoc.CreateElement("Position"));
            ((XmlElement)position).SetAttribute("x", prop.transform.position.x.ToString());
            ((XmlElement)position).SetAttribute("y", prop.transform.position.y.ToString());
            ((XmlElement)position).SetAttribute("z", prop.transform.position.z.ToString());
            var euler = node.AppendChild(xDoc.CreateElement("Euler"));
            ((XmlElement)euler).SetAttribute("x", prop.transform.eulerAngles.x.ToString());
            ((XmlElement)euler).SetAttribute("y", prop.transform.eulerAngles.y.ToString());
            ((XmlElement)euler).SetAttribute("z", prop.transform.eulerAngles.z.ToString());

            yield return null;
        }

        var meta = xDoc.DocumentElement.AppendChild(xDoc.CreateElement("Meta"));
        ((XmlElement)meta).SetAttribute("CreationDate", System.DateTime.Now.ToString());
        yield return null;

        var levels = Application.streamingAssetsPath + "/levels";
        if (!System.IO.Directory.Exists(levels)) System.IO.Directory.CreateDirectory(levels);
        xDoc.Save(levels + "/" + name + ".xml");


        saveProgressionScreen.SetActive(false);
        yield return true;
    }

    public float GetSavingProgression()
    {
        return savingAdvancement;
    }

    public static void LoadLevel(string levelPath)
    {
        var xDoc = new XmlDocument();
        xDoc.Load(levelPath);
        var props = GetPropsFromXDoc(xDoc);
        foreach(var prop in props) {
            if (prop.prefab.GetComponent<Prop>().isHuman) {
                Instantiate(Library.instance.humanPrefab, prop.position, Quaternion.identity);
                continue;
            }
            Instantiate(prop.prefab, prop.position, Quaternion.Euler(prop.euler));
        }
    }

    static List<SavedProp> GetPropsFromXDoc(XmlDocument xDoc)
    {
        var props = new List<SavedProp>();
        var xNode = xDoc.GetElementsByTagName("Props")[0];
        // For each prop
        foreach (var prop in xNode.ChildNodes) {
            var xProp = (XmlElement)prop;

            foreach (var lProp in Library.instance.props) {
                var lPropComp = lProp.GetComponent<Prop>();
                if (lPropComp == null) continue;

                if (System.Convert.ToInt32(xProp.Attributes["id"].Value) == lPropComp.id) {
                    // Only if it exists in the library
                    var position = new Vector3();
                    var euler = new Vector3();
                    foreach (var childNode in xProp.ChildNodes) {
                        // Parsing the child nodes of prop (position, ...)
                        var xChildNode = (XmlElement)childNode;
                        switch (xChildNode.Name) {
                            case "Position":
                                position = new Vector3(
                                    System.Convert.ToSingle(xChildNode.Attributes["x"].Value),
                                    System.Convert.ToSingle(xChildNode.Attributes["y"].Value),
                                    System.Convert.ToSingle(xChildNode.Attributes["z"].Value)
                                ); break;

                            case "Euler":
                                euler = new Vector3(
                                    System.Convert.ToSingle(xChildNode.Attributes["x"].Value),
                                    System.Convert.ToSingle(xChildNode.Attributes["y"].Value),
                                    System.Convert.ToSingle(xChildNode.Attributes["z"].Value)
                                ); break;
                        }
                    }

                    // adding it to the list of things to be spawned
                    props.Add(new SavedProp() { prefab = lProp, position = position });
                    break;
                }
            }
        }

        return props;
    }

}
