using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XInputDotNetPure;

public class LevelEditor : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI title;

    int currentCursor = 0;
    Dictionary<Image, GameObject> entries = new Dictionary<Image, GameObject>();
    bool canMoveInMenus = true;
    bool isMenuing = false;

    private void Start()
    {
        StartCoroutine(CreatePreviews());
    }

    private void Update()
    {
        UpdateVisuals();
        if (isMenuing)
        {
            CheckInputs();
            UpdateSelection();
        }
    }

    void UpdateVisuals()
    {
        container.SetActive(isMenuing);
        title.gameObject.SetActive(isMenuing);
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

    void CheckInputs()
    {
        var state = GamePad.GetState(GameManager.instance.GetPlayers()[0].index);
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

        if (state.Buttons.A == ButtonState.Pressed)
        {

            isMenuing = false;
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

}
