// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>
using UnityEngine;
using static UnityEngine.Mathf;



namespace LumenSection.LevelLinker
{
public class Character : MonoBehaviour
{
  public enum AxisState
  {
    MIDDLE,
    TOP,
    BOTTOM
  }



  public class KeyAxis
  {
    private AxisState mState;
    private bool      mNegativePressed;
    private bool      mPositivePressed;
    private bool      mWentUp;
    private bool      mWentDown;



    public KeyAxis()
    {
      mState           = AxisState.MIDDLE;
      mNegativePressed = false;
      mPositivePressed = false;
      mWentUp          = false;
      mWentDown        = false;
    }

    public float GetPosition()
    {
      switch (mState)
      {
        case AxisState.MIDDLE: return 0f;
        case AxisState.BOTTOM: return -1f;
        case AxisState.TOP:    return 1f;
      }
      return 0f;
    }

    public bool IsDown()
    {
      return mState == AxisState.BOTTOM;
    }

    public bool IsUp()
    {
      return mState == AxisState.TOP;
    }

    public bool WentDown()
    {
      return mWentDown;
    }

    public bool WentUp()
    {
      return mWentUp;
    }

    public void Reset()
    {
      mState           = AxisState.MIDDLE;
      mNegativePressed = false;
      mPositivePressed = false;
      mWentUp          = false;
      mWentDown        = false;
    }

    public void Update(KeyCode positiveKey, KeyCode negativeKey)
    {
      mWentDown = false;
      mWentUp   = false;
      bool      positiveDown = Input.GetKey(positiveKey);
      bool      negativeDown = Input.GetKey(negativeKey);
      AxisState current      = mState;
      if (positiveDown && !mPositivePressed)
      {
        mState           = AxisState.TOP;
        mPositivePressed = true;
      }
      if (!positiveDown && mPositivePressed)
      {
        if (mNegativePressed)
          mState = AxisState.BOTTOM;
        else
          mState = AxisState.MIDDLE;
        mPositivePressed = false;
      }
      if (negativeDown && !mNegativePressed)
      {
        mState           = AxisState.BOTTOM;
        mNegativePressed = true;
      }
      if (!negativeDown && mNegativePressed)
      {
        if (mPositivePressed)
          mState = AxisState.TOP;
        else
          mState = AxisState.MIDDLE;
        mNegativePressed = false;
      }
      if (current is AxisState.MIDDLE or AxisState.TOP && mState == AxisState.BOTTOM)
        mWentDown = true;
      if (current is AxisState.MIDDLE or AxisState.BOTTOM && mState == AxisState.TOP)
        mWentUp = true;
    }
  }



  // Components
  private Rigidbody2D      mBody;
  private CircleCollider2D mCollider;
  private Animator         mAnimator;

  // Internals
  private KeyAxis mHorizontalAxis;
  private KeyAxis mVerticalAxis;



  private void Awake()
  {
    // Don't destroy when scene unloads
    DontDestroyOnLoad(gameObject);

    // Get components
    mBody     = GetComponent<Rigidbody2D>();
    mCollider = GetComponent<CircleCollider2D>();
    mAnimator = GetComponent<Animator>();

    // Axis
    mHorizontalAxis = new KeyAxis();
    mVerticalAxis   = new KeyAxis();
  }

  public float Radius
  {
    get
    {
      return mCollider.radius;
    }
  }

  private void OnEnable()
  {
    // Clear velocity
    mBody.velocity = Vector2.zero;
    mAnimator.SetBool("Walking", false);
    mHorizontalAxis.Reset();
    mVerticalAxis.Reset();
  }

  private void Update()
  {
    mHorizontalAxis.Update(KeyCode.RightArrow, KeyCode.LeftArrow);
    mVerticalAxis.Update(KeyCode.UpArrow, KeyCode.DownArrow);
  }

  private static int IntDirection(float axis)
  {
    if (Approximately(axis, 0f))
      return 0;
    return (int)Sign(axis);
  }

  private void FixedUpdate()
  {
    // Compute velocity depending on inputs
    float   hAxis     = mHorizontalAxis.GetPosition();
    float   vAxis     = mVerticalAxis.GetPosition();
    Vector2 direction = Vector2.right * hAxis + Vector2.up * vAxis;

    // Apply velocity to rigid body
    const float moveVelocity = 5f;
    mBody.velocity = direction * moveVelocity;

    // Update animator depending on velocity
    mAnimator.SetInteger("Horizontal", IntDirection(hAxis));
    mAnimator.SetInteger("Vertical",   IntDirection(vAxis));
    mAnimator.SetBool("Walking", Abs(hAxis) > 0.1f || Abs(vAxis) > 0.1f);
  }

  public void SetDirection(Vector2 direction)
  {
    mAnimator.SetInteger("Horizontal", IntDirection(direction.x));
    mAnimator.SetInteger("Vertical",   IntDirection(direction.y));
  }
}
}
