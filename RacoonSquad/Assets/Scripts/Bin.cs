/*
public int id;
// LEFT JOYSTICK
private string leftHorizontalAxisName;
private float leftHorizontalAxisValue;
private string leftVerticalAxisName;
private float leftVerticalAxisValue;
private string leftJoystickButton;
// RIGHT JOYSTICK
private string rightHorizontalAxisName;
private float rightHorizontalAxisValue;
private string rightVerticalAxisName;
private float rightVerticalAxisValue;
private string rightJoystickButton;
// PAD ARROW
private string padUpName;
private string padRightName;
private string padDownName;
private string padLeftName;
// TRIGGERS
private string rightTriggerAxisName;
private float rightTriggerAxisValue;
private string leftTriggerAxisName;
private float leftTriggerAxisValue;

private string rightBumperName;
private string leftBumperName;
// BUTTONS
private string aButtonName;
private string bButtonName;
private string yButtonName;
private string xButtonName;
private string menuButtonName;
private string selectButtonName;

void Set(int index)
{
    id = index;

    leftHorizontalAxisName = id + "_Left_Horizontal";
    leftVerticalAxisName = id + "_Left_Vertical";
    leftJoystickButton = id + "_Left_JoystickButton";

    rightHorizontalAxisName = id + "_Right_Horizontal";
    rightVerticalAxisName = id + "_Right_Vertical";
    rightJoystickButton = id + "_Right_JoystickButton";

    
    padUpName = id + "_Up_Pad";
    padRightName = id + "_Right_Pad";
    padDownName = id + "_Down_Pad";
    padLeftName = id + "_Left_Pad";

    rightTriggerAxisName = id + "_Right_Trigger";
    leftTriggerAxisName = id + "_Left_Trigger";

    rightBumperName = id + "_Right_Bumper";
    leftBumperName = id + "_Left_Bumper";

    aButtonName = id + "_aButton";
    bButtonName = id + "_bButton";
    yButtonName = id + "_yButton";
    xButtonName = id + "_xButton";
    menuButtonName = id + "_Menu";
    selectButtonName = id + "_Select";
}

void Awake()
{
    Set(id);
}

void Update()
{
    if(Input.GetButtonDown(aButtonName)) Debug.Log(aButtonName);
}

void GetInputs()
{
    leftHorizontalAxisValue = Input.GetAxis("leftHorizontalAxisName");
    leftVerticalAxisValue = Input.GetAxis("leftVerticalAxisName");

    rightTriggerAxisValue = Input.GetAxis()
}
*/