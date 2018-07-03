using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WakaManView : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 7;
    [SerializeField] private Tilemap _wallsTilemap;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private Vector3Int _currentCellPos;

    private TileBase _topTile;
    private TileBase _bottomTile;
    private TileBase _rightTile;
    private TileBase _leftTile;

    private enum Position
    {
        Left,
        Right,
        Top,
        Bottom,
        Center,
        Horizontal,
        Vertical,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [SerializeField] private Position _currentPosition;

    [SerializeField] private Vector2 _move = Vector2.zero;

    // Use this for initialization
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        var direction = Vector2.zero;
        direction.x = Input.GetAxis("Horizontal");
        direction.y = Input.GetAxis("Vertical");

        _move = GetCurrentMoveVector(direction);
        InitNeighboringTiles();
        _currentPosition = GetCurrentPosition();

        var vectorMove = GetMoveVector(direction);

        transform.position = new Vector2(transform.position.x + vectorMove.x, transform.position.y + vectorMove.y);
    }

    /// <summary>
    /// Get move vector
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private Vector2 GetMoveVector(Vector2 direction)
    {
        Vector2 vectorMove;

        switch (_currentPosition)
        {
            case Position.Left:
                InitMove(out vectorMove, direction, Position.Center, Position.Left);
                break;
            case Position.Right:
                InitMove(out vectorMove, direction, Position.Center, Position.Right);
                break;
            case Position.Top:
                InitMove(out vectorMove, direction, Position.Top, Position.Center);
                break;
            case Position.Bottom:
                InitMove(out vectorMove, direction, Position.Bottom, Position.Center);
                break;
            case Position.Center:
                InitMove(out vectorMove, direction, Position.Center, Position.Center);
                break;
            case Position.Horizontal:
                InitHorizontalMove(out vectorMove);
                break;
            case Position.Vertical:
                InitVerticalMove(out vectorMove);
                break;
            case Position.TopLeft:
                InitMove(out vectorMove, direction, Position.Top, Position.Left);
                break;
            case Position.TopRight:
                InitMove(out vectorMove, direction, Position.Top, Position.Right);
                break;
            case Position.BottomLeft:
                InitMove(out vectorMove, direction, Position.Bottom, Position.Left);
                break;
            case Position.BottomRight:
                InitMove(out vectorMove, direction, Position.Bottom, Position.Right);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return vectorMove;
    }

    /// <summary>
    /// Init move
    /// </summary>
    /// <param name="vectorMove"></param>
    /// <param name="direction"></param>
    /// <param name="vertical"></param>
    /// <param name="horizontal"></param>
    private void InitMove(out Vector2 vectorMove, Vector2 direction, Position vertical, Position horizontal)
    {
        bool verticalCheck;

        switch (vertical)
        {
            case Position.Bottom:
                verticalCheck = direction.y > 0;
                break;
            case Position.Top:
                verticalCheck = direction.y < 0;
                break;
            default:
                verticalCheck = Math.Abs(direction.y) > 0;
                break;
        }

        bool horizontalCheck;

        switch (horizontal)
        {
            case Position.Right:
                horizontalCheck = direction.x < 0;
                break;
            case Position.Left:
                horizontalCheck = direction.x > 0;
                break;
            default:
                horizontalCheck = Math.Abs(direction.x) > 0;
                break;
        }

        if (verticalCheck)
        {
            InitVerticalMove(out vectorMove);
        }
        else if (horizontalCheck)
        {
            InitHorizontalMove(out vectorMove);
        }
        else
        {
            InitCenterPos(out vectorMove);
        }
    }

    /// <summary>
    /// Init current position
    /// </summary>
    private Position GetCurrentPosition()
    {
        Position position;
        if (_topTile && _bottomTile)
        {
            position = Position.Horizontal;
        }
        else if (_rightTile && _leftTile)
        {
            position = Position.Vertical;
        }
        else if (_rightTile && _topTile)
        {
            position = Position.TopRight;
        }
        else if (_leftTile && _topTile)
        {
            position = Position.TopLeft;
        }
        else if (_leftTile && _bottomTile)
        {
            position = Position.BottomLeft;
        }
        else if (_rightTile && _bottomTile)
        {
            position = Position.BottomRight;
        }
        else if (_topTile)
        {
            position = Position.Top;
        }
        else if (_bottomTile)
        {
            position = Position.Bottom;
        }
        else if (_leftTile)
        {
            position = Position.Left;
        }
        else if (_rightTile)
        {
            position = Position.Right;
        }
        else
        {
            position = Position.Center;
        }

        return position;
    }


    /// <summary>
    /// Init move vector
    /// </summary>
    /// <param name="direction"></param>
    private Vector2 GetCurrentMoveVector(Vector2 direction)
    {
        var moveVector = _move;
        if (direction.x > 0.01f)
        {
            moveVector.x = _maxSpeed;
            if (_spriteRenderer.flipX)
            {
                _spriteRenderer.flipX = false;
            }
        }
        else if (direction.x < -0.01f)
        {
            moveVector.x = -_maxSpeed;
            if (_spriteRenderer.flipX == false)
            {
                _spriteRenderer.flipX = true;
            }
        }

        if (direction.y > 0.01f)
        {
            moveVector.y = _maxSpeed;
        }
        else if (direction.y < -0.01f)
        {
            moveVector.y = -_maxSpeed;
        }

        return moveVector;
    }

    /// <summary>
    /// Init neighboring tiles
    /// </summary>
    private void InitNeighboringTiles()
    {
        var playerPos = _wallsTilemap.WorldToCell(transform.position);

        if (_currentCellPos == playerPos) return;

        const float radius = 0.5f;
        _currentCellPos = playerPos;

        _topTile = GetTilemapGridCell(
            new Vector2(transform.position.x, transform.position.y + radius)
        );
        _bottomTile = GetTilemapGridCell(
            new Vector2(transform.position.x, transform.position.y - radius)
        );
        _rightTile = GetTilemapGridCell(
            new Vector2(transform.position.x + radius, transform.position.y)
        );
        _leftTile = GetTilemapGridCell(
            new Vector2(transform.position.x - radius, transform.position.y)
        );
    }

    /// <summary>
    /// Init center pos
    /// </summary>
    /// <param name="vectorMove"></param>
    private void InitCenterPos(out Vector2 vectorMove)
    {
        vectorMove = Vector2.zero;
        transform.position = GetCenterPos();
    }

    /// <summary>
    /// Init horizontal move
    /// </summary>
    /// <param name="vectorMove"></param>
    private void InitHorizontalMove(out Vector2 vectorMove)
    {
        vectorMove = _move * Time.deltaTime;
        vectorMove.y = 0;
        transform.position = new Vector2(transform.position.x, GetCenterPos().y);
    }

    /// <summary>
    /// Init vertical move
    /// </summary>
    /// <param name="vectorMove"></param>
    private void InitVerticalMove(out Vector2 vectorMove)
    {
        vectorMove = _move * Time.deltaTime;
        vectorMove.x = 0;
        transform.position = new Vector2(GetCenterPos().x, transform.position.y);
    }


    /// <summary>
    /// Get Center Cell position
    /// </summary>
    /// <returns></returns>
    private Vector2 GetCenterPos()
    {
        var gridPos = _wallsTilemap.CellToWorld(_currentCellPos);
        return new Vector2(gridPos.x + 0.24f, gridPos.y + 0.24f);
    }


    /// <summary>
    /// Get tilemap grid cell
    /// </summary>
    /// <returns></returns>
    private TileBase GetTilemapGridCell(Vector2 pos)
    {
        var originCell = _wallsTilemap.WorldToCell(pos);
        return _wallsTilemap.GetTile(originCell);
    }
}