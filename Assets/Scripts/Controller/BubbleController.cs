using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace com.javierquevedo
{
	public class BubbleController : MonoBehaviour
	{
		public Bubble bubble;
		public float leftBorder; 
		public float rightBorder; 
		public float topBorder; 
		public float linearSpeed; 
		public float radius;
		public float angle; 
		public bool isMoving;
		private const float _killSpeed = 10.0f;
		MotionDetectionDelegate motionDelegate;
		public delegate bool MotionDetectionDelegate(Vector3 position);
		CollisionDetectionDelegate collisionDelegate;
		public delegate void CollisionDetectionDelegate(GameObject bubble);
		public CollisionDetectionDelegate CollisionDelegate
		{
			set
			{
				collisionDelegate = value;
			}
		}
		public MotionDetectionDelegate MotionDelegate
		{
			set
			{
				motionDelegate = value;
			}
		}
		void Awake()
		{
			bubble = new Bubble(JQUtils.GetRandomEnum<BubbleColor>());
		}
		void Start()
		{
			this.GetComponent<Renderer>().material.color = JQUtils.ColorForBubbleColor(bubble.color);
			string endereco = JQUtils.enderecoSprite(bubble.color);
			MakeInvisible();
			CreateChildObject(endereco); 
		}
		void CreateChildObject(string endereco)
		{
			GameObject spriteobj = new GameObject("Spriteobj");
			spriteobj.transform.parent = transform;
			MeshRenderer parentMeshRenderer = GetComponent<MeshRenderer>();
			SpriteRenderer childSpriteRenderer = spriteobj.AddComponent<SpriteRenderer>();
			childSpriteRenderer.sprite = Resources.Load<Sprite>(endereco);
			spriteobj.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
			spriteobj.transform.localPosition = new Vector3(0f, 0f, 0f);
			MakeVisible(spriteobj);
		}
		void MakeVisible(GameObject obj)
		{
			SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				spriteRenderer.enabled = true;
			}
			else
			{
				obj.SetActive(true);
			}
		}
		void MakeInvisible()
		{
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
		void Update()
		{
			if (isMoving)
			{
				this.transform.Translate(Vector3.right * this.linearSpeed * Mathf.Cos(Mathf.Deg2Rad * this.angle) * Time.deltaTime);
				this.transform.Translate(Vector3.up * this.linearSpeed * Mathf.Sin(Mathf.Deg2Rad * this.angle) * Time.deltaTime);
				if (this.motionDelegate != null)
				{
					if (!this.motionDelegate(this.transform.position))
					{
						this.transform.Translate(Vector3.left * this.linearSpeed * Mathf.Cos(Mathf.Deg2Rad * this.angle) * Time.deltaTime);
						this.transform.Translate(Vector3.down * this.linearSpeed * Mathf.Sin(Mathf.Deg2Rad * this.angle) * Time.deltaTime);
						this.isMoving = false;
						if (collisionDelegate != null)
						{
							collisionDelegate(this.gameObject);
						}
					}
					else
					{
						this.updateDirection();
					}
				}
			}
		}
		public void kill(bool explodes)
		{
			StopAllCoroutines();
			Destroy(this.transform.GetComponent<Rigidbody>());
			Destroy(this.transform.GetComponent<Collider>());
			if (explodes)
			{
				StartCoroutine(scaleTo(new Vector3(0, 0, 0), 0.15f));
			}
			else
			{
				Vector3 killPosition = new Vector3(this.transform.position.x, 0f, 0f);
				float distance = Vector3.Distance(this.transform.position, killPosition);
				this.moveTo(killPosition, distance / _killSpeed);
			}
		}
		public void moveTo(Vector3 destination, float duration)
		{
			StartCoroutine(tweenTo(destination, duration));
		}
		IEnumerator tweenTo(Vector3 destination, float duration)
		{
			float timeThrough = 0.0f;
			Vector3 initialPosition = transform.position;
			while (Vector3.Distance(transform.position, destination) >= 0.05)
			{
				timeThrough += Time.deltaTime;
				Vector3 target = Vector3.Lerp(initialPosition, destination, timeThrough / duration);
				transform.position = target;
				yield return null;
			}
			transform.position = destination;
			if (this.GetComponent<Rigidbody>() == null)
			{
				Destroy(this.gameObject);
			}
		}
		IEnumerator scaleTo(Vector3 scale, float duration)
		{
			float timeThrough = 0.0f;
			Vector3 initialScale = transform.localScale;
			while (transform.localScale.x >= 0.1)
			{
				timeThrough += Time.deltaTime;
				Vector3 target = Vector3.Lerp(initialScale, scale, timeThrough / duration);
				transform.localScale = target;
				yield return null;
			}
			if (this.GetComponent<Rigidbody>() == null)
			{
				Destroy(this.gameObject);
			}
		}
		void OnTriggerEnter(Collider collider)
		{
			if (this.isMoving)
			{
				this.isMoving = false;
				if (collisionDelegate != null)
				{
					collisionDelegate(this.gameObject);
				}
			}
		}
		void updateDirection()
		{
			if (this.transform.position.x + this.radius >= this.rightBorder || this.transform.position.x - this.radius <= this.leftBorder)
			{
				this.angle = 180.0f - this.angle;
				if (this.transform.position.x + this.radius >= this.rightBorder)
					this.transform.position = new Vector3(this.rightBorder - this.radius, this.transform.position.y, this.transform.position.z);
				if (this.transform.position.x - this.radius <= this.leftBorder)
					this.transform.position = new Vector3(this.leftBorder + this.radius, this.transform.position.y, this.transform.position.z);
			}
			if (this.transform.position.y + this.radius >= this.topBorder)
			{
				this.isMoving = false;
				if (collisionDelegate != null)
				{
					collisionDelegate(this.gameObject);
				}
			}
		}
	}
}