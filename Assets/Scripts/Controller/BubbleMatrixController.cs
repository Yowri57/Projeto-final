using UnityEngine;
using System.Collections;
using com.javierquevedo.events;
namespace com.javierquevedo
{
	public class BubbleMatrixController : MonoBehaviour
	{
		public float leftBorder = 0.0f;
		public float rightBorder = 10.5f;
		public float topBorder = 10.0f;
		public int rows = 4;
		public int columns = 10;
		public float bubbleRadius = 0.5f;
		public float addRowPeriod = 20.0f;
		public BubbleMatrixGeometry geometry;
		private const float _bubbleLinearSpeed = 12.0f;
		private const string _bubblePrefabName = "Prefabs/BubblePrefab";
		private const int _defaultRowsCount = 3;
		public GameObject Mostrador;
		private BubbleMatrix _matrix;
		private GameObject _bubblesContainer;
		private GameObject _bubbleShooter;
		private BubbleController _currentBubble;
		private ArrayList _bubbleControllers;
		private bool _pendingToAddRow;
		private bool _isPlaying;
		private GetColor.Cor corAtual;
		public SpriteRenderer spriteRenderer;
		void Awake()
		{
			this._isPlaying = false;
			this._pendingToAddRow = false;
			this._matrix = new BubbleMatrix(rows, columns);
			this._bubbleControllers = new ArrayList();
			CreateMostrador();
		}
		void Start()
		{
		}
		public void startGame()
		{
			this._bubblesContainer = GameObject.Find("Bubbles");
			this._bubbleShooter = GameObject.Find("BubbleShooter");
			this.geometry = new BubbleMatrixGeometry(leftBorder, rightBorder, topBorder, 0.0f, rows, columns, bubbleRadius);
			this._currentBubble = this.createBubble();
			this._isPlaying = true;
			StartCoroutine("addRowScheduler");
			for (int i = 0; i < _defaultRowsCount; i++)
			{
				this.addRow();
			}
		}
		void Update()
		{
			if (Input.GetMouseButtonDown(0) && this._isPlaying)
			{
				if (this._currentBubble != null)
				{
					this._currentBubble.isMoving = true;
					this._currentBubble.angle = this.shootingRotation();
					this._currentBubble = null;
				}
			}

		}
		private BubbleController createBubble()
		{
			GameObject bubblePrefab = Instantiate(Resources.Load(_bubblePrefabName)) as GameObject;
			bubblePrefab.transform.parent = _bubblesContainer.transform;
			bubblePrefab.transform.position = new Vector3((rightBorder - leftBorder) / 2.0f - geometry.bubbleRadius / 2.0f, -0.65f, 0);
			BubbleController bubbleController = bubblePrefab.GetComponent<BubbleController>();
			bubbleController.leftBorder = this.geometry.leftBorder;
			bubbleController.rightBorder = this.geometry.rightBorder;
			bubbleController.topBorder = this.geometry.topBorder;
			bubbleController.radius = this.geometry.bubbleRadius;
			bubbleController.linearSpeed = _bubbleLinearSpeed;
			bubbleController.angle = 90.0f;
			bubbleController.isMoving = false;
			bubbleController.CollisionDelegate = onBubbleCollision;
			bubbleController.MotionDelegate = canMoveToPosition;
			this._bubbleControllers.Add(bubbleController);
			return bubbleController;
		}
		public void CreateMostrador()
		{
			Mostrador = new GameObject("Mostrador");
			Mostrador.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
			Mostrador.transform.position = new Vector3(13.4f, 6f, 0f);
			spriteRenderer = Mostrador.AddComponent<SpriteRenderer>();
			attmostrador();
		}
		public void attmostrador()
		{
			corAtual = (GetColor.Cor)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(GetColor.Cor)).Length);
			string nomeCor = corAtual.ToString();
			string spritePath = "Sprites/Bubbles/" + nomeCor + "BubbleSprite";
			spriteRenderer.sprite = Resources.Load<Sprite>(spritePath);
		}
		private float shootingRotation()
		{
			float shooterRotation = this._bubbleShooter.transform.eulerAngles.z;
			float ballRotation = 90;
			if (shooterRotation <= 360 && shooterRotation >= 270.0)
			{
				ballRotation = shooterRotation - 270;
			}
			if (shooterRotation <= 90 && shooterRotation >= 0)
			{
				ballRotation = 90 + shooterRotation;
			}
			return ballRotation;
		}
		private void destroyBubble(BubbleController bubbleController, bool explodes)
		{
			this._matrix.remove(bubbleController.bubble);
			this._bubbleControllers.Remove(bubbleController);
			bubbleController.CollisionDelegate = null;
			bubbleController.kill(explodes);
		}
		private BubbleController controllerForBubble(Bubble bubble)
		{
			foreach (BubbleController bubbleController in this._bubbleControllers)
			{
				if (bubbleController.bubble == bubble)
					return bubbleController;
			}
			return null;
		}
		IEnumerator addRowScheduler()
		{
			yield return new WaitForSeconds(addRowPeriod);
			this._pendingToAddRow = true;
		}
		private void destroyCluster(ArrayList cluster, bool explodes)
		{
			foreach (Bubble bubble in cluster)
			{
				this.destroyBubble(this.controllerForBubble(bubble), explodes);
			}
		}
		private void addRow()
		{
			this._pendingToAddRow = false;
			bool overflows = this._matrix.shiftOneRow();
			for (int i = 0; i < this.geometry.columns; i++)
			{
				BubbleController bubbleController = this.createBubble();
				bubbleController.isMoving = false;
				this._matrix.insert(bubbleController.bubble, 0, i);
			}
			foreach (BubbleController bubbleController in this._bubbleControllers)
			{
				if (bubbleController != this._currentBubble)
				{
					Vector3 position = BubbleMatrixControllerHelper.PositionForCell(this._matrix.location(bubbleController.bubble), geometry, this._matrix.isBaselineAlignedLeft);
					bubbleController.transform.position = position;
				}
			}
			if (overflows)
			{
				this.FinishGame(GameState.Loose);
				return;
			}
		}
		private void FinishGame(GameState state)
		{
			BubbleShooterController shooterController = this._bubbleShooter.GetComponent<BubbleShooterController>();
			shooterController.isAiming = false;
			GameEvents.GameFinished(state);
			this._isPlaying = false;
		}
		void onBubbleCollision(GameObject bubble)
		{
			Vector2 bubblePos = BubbleMatrixControllerHelper.CellForPosition(bubble.transform.position, this.geometry, this._matrix.isBaselineAlignedLeft);
			if ((int)bubblePos.x >= this.geometry.rows)
			{
				this.FinishGame(GameState.Loose);
				return;
			}
			BubbleController bubbleController = bubble.GetComponent<BubbleController>();
			Vector2 matrixPosition = BubbleMatrixControllerHelper.CellForPosition(bubble.transform.position, this.geometry, this._matrix.isBaselineAlignedLeft);
			this._matrix.insert(bubbleController.bubble, (int)matrixPosition.x, (int)matrixPosition.y);
			if (!this._pendingToAddRow)
			{
				bubbleController.moveTo(BubbleMatrixControllerHelper.PositionForCell(matrixPosition, geometry, this._matrix.isBaselineAlignedLeft), 0.1f);
			}
			else
			{
				bubbleController.transform.position = BubbleMatrixControllerHelper.PositionForCell(matrixPosition, geometry, this._matrix.isBaselineAlignedLeft);
			}
			ArrayList cluster = this._matrix.colorCluster(bubbleController.bubble);
			if (cluster.Count > 2 && bubbleController.bubble.color == (BubbleColor)corAtual)
			{
				bubbleController.transform.position = BubbleMatrixControllerHelper.PositionForCell(matrixPosition, geometry, this._matrix.isBaselineAlignedLeft);
				this.destroyCluster(cluster, true);
				GameEvents.BubblesRemoved(cluster.Count, true);
			}
			cluster = this._matrix.looseBubbles();
			this.destroyCluster(cluster, false);
			if (cluster.Count > 0)
				GameEvents.BubblesRemoved(cluster.Count, false);
			if (_pendingToAddRow)
			{
				this.addRow();
				StartCoroutine("addRowScheduler");
			}
			if (this._matrix.bubbles.Count == 0)
			{
				this.FinishGame(GameState.Win);
				return;
			}
			attmostrador();
			this._currentBubble = this.createBubble();
		}
		bool canMoveToPosition(Vector3 position)
		{
			Vector2 location = BubbleMatrixControllerHelper.CellForPosition(position, this.geometry, this._matrix.isBaselineAlignedLeft);
			if ((int)location.x <= this.geometry.rows - 1)
			{
				return !this._matrix.hasBubble((int)location.x, (int)location.y);
			}
			return true;
		}
	}
}
