using Godot;

namespace RandomWorldGen;

public partial class ZoomCamera2D : Camera2D
{
	[Export()] private float _minZoom = 0.5f;
	[Export()] private float _maxZoom = 2f;
	[Export()] private float _zoomFactor = 0.1f;
	[Export()] private float _zoomDuration = 0.2f;
	[Export()] private float _moveSpeedMultiplier = 8;

	private float _zoomLevel = 1;
	private float _moveVelocity = 1;
	
	public override void _Process(double delta)
	{
		MoveCamera();
	}

	private void MoveCamera()
	{
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
		_moveVelocity = _moveSpeedMultiplier / _zoomLevel;
		Position += inputDirection * _moveVelocity;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse)
		{
			if ( mouse.ButtonIndex == MouseButton.WheelUp)
			{
				SetZoomLevel(_zoomLevel - _zoomFactor);
			}
			if ( mouse.ButtonIndex == MouseButton.WheelDown)
			{
				SetZoomLevel(_zoomLevel + _zoomFactor);
			}
		}
	}

	private void SetZoomLevel(float value)
	{
		_zoomLevel = Mathf.Clamp(value, _minZoom, _maxZoom);
		var tween = GetTree().CreateTween().BindNode(this).SetTrans(Tween.TransitionType.Sine)
			.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, "zoom", new Vector2(_zoomLevel, _zoomLevel), _zoomDuration);
		tween.Play();

	}
	
}