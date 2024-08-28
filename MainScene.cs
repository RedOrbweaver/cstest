using System.Collections.Specialized;
using System.ComponentModel;
using Godot;
using System;

[Tool]
public class MainScene : Control
{
    Control _background;
    const string SHADER_COLOR_MAP = "color_map";
    const string SHADER_MAX_ITERATIONS = "max_iterations";
    const string SHADER_OFFSET = "offset";
    const string SHADER_ZOOM = "zoom"; 
    const string SHADER_DIVERGENCE_THRESHOLD = "divergence_threshold";

    const float _zoomPerTick = 1.2f;
    const float _movementRate = 0.001f;

    float _zoom = 1.0f; 
    [Export] 
    public float Zoom
    {
        get => _zoom;
        set {_zoom = value; UpdateShader();}
    }
    Vector2 _offset = new Vector2(0,0);
    [Export] 
    public Vector2 Offset
    {
        get => _offset;
        set {_offset = value; UpdateShader();}
    }
    float _realZoom = 1.0f;
    bool _idleAnimation = true;
    [Export]
    public bool IdleAnimation
    {
        get => _idleAnimation;
        set
        {
            _idleAnimation = value;
            SetProcess(_idleAnimation || !Engine.EditorHint);
            if(!_idleAnimation)
                Zoom = _realZoom;
            _realZoom = Zoom;
            UpdateShader();
        }
    }
    bool _updateQueued = false; 
    void UpdateShader()
    {
        if(_updateQueued)
            return;  
        _updateQueued = true;
        CallDeferred("RealUpdateShader");
    }
    void RealUpdateShader()
    {    
        _updateQueued = false;
        ShaderMaterial mat = (ShaderMaterial)_background.Material;
        mat.SetShaderParam(SHADER_OFFSET, Offset);
        mat.SetShaderParam(SHADER_ZOOM, Zoom);  
    }
    
    public override void _Ready()
    {
        _realZoom = _zoom;
        _updateQueued = false;
        _background = GetNode<Control>("ViewportContainer/Viewport/Background");
        RealUpdateShader();
    }
    int dir = 1;
    public override void _Process(float delta)
    {
        if(Engine.EditorHint)
        {
            if(dir == 1)
            {
                _zoom = Mathf.Lerp(_zoom, _zoom * _zoomPerTick, delta*10.0f);
                if(Zoom > 300.0f)
                    dir = -1;
            }
            else
            {
                _zoom = Mathf.Lerp(_zoom, _zoom * (1.0f/_zoomPerTick), delta*10.0f);
                if(Zoom < 0.3)
                    dir = 1;
            }
            UpdateShader();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton iemb)
        {
            if(iemb.ButtonIndex == (int)ButtonList.WheelUp)
            {
                Zoom *= _zoomPerTick;
            }
            else if(iemb.ButtonIndex == (int)ButtonList.WheelDown)
            {
                Zoom *= 1.0f/(_zoomPerTick);
            }
        }
        else if(@event is InputEventMouseMotion iemm)
        {
            if(Input.IsMouseButtonPressed((int)ButtonList.Left))
            {
                Offset += _movementRate*iemm.Relative/Zoom;
            }
        }
    }   
 
    
}
