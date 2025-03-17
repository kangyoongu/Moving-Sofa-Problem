using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kang
{
    public class AgentController : MonoBehaviour
    {
        private ConstantForce2D constForce;
        private Material material;
        [HideInInspector] public Rigidbody2D rigid;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public PolygonCollider2D col;
        public float score;
        void Awake()
        {
            material = GetComponent<MeshRenderer>().material;
            constForce = GetComponent<ConstantForce2D>();
            meshFilter = GetComponent<MeshFilter>();
            rigid = GetComponent<Rigidbody2D>();
            col = GetComponent<PolygonCollider2D>();
        }
        public void Init()
        {
            transform.position = AgentManager.Instance.startPos.position;
            transform.rotation = Quaternion.identity;
            rigid.velocity = Vector2.zero;
            rigid.angularVelocity = 0f;

            constForce.force = new Vector2(AgentManager.Instance.moveForce, 0f);
            constForce.torque = 0f;
            material.color = Core.RandomColor();

            Vector2[] mut = Core.Mutation(AgentManager.Instance.bestVectors, AgentManager.Instance.power, AgentManager.Instance.ratio, AgentManager.Instance.vecRatio);
            col.SetPath(0, mut);

            meshFilter.mesh = AgentManager.Instance.GetMesh(col, transform);
            score = Core.CalculateMeshArea(meshFilter.mesh);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.name == "LeftWall")
            {
                constForce.torque = AgentManager.Instance.torque;
                constForce.force = new Vector2(0f, -AgentManager.Instance.moveForce);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.name == "Goal")
            {
                AgentManager.Instance.Goal(this);
                gameObject.SetActive(false);
            }
        }
    }
}
