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
        int forceIndex = 0;
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

            Vector2[] mut = Core.Mutation(AgentManager.Instance.bestVectors, AgentManager.Instance.power, AgentManager.Instance.ratio, AgentManager.Instance.vecARatio, AgentManager.Instance.vecNRatio);
            col.SetPath(0, mut);

            var mesh = AgentManager.Instance.GetMesh(col, transform);
            meshFilter.mesh = mesh;
            score = Core.CalculateMeshArea(meshFilter.mesh) + mesh.vertices.Length * AgentManager.Instance.vertCountWeight;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(forceIndex < AgentManager.Instance.sofaForces.Count && collision.gameObject.name == AgentManager.Instance.sofaForces[forceIndex].name)
            {
                constForce.torque = AgentManager.Instance.sofaForces[forceIndex].turque;
                constForce.force = AgentManager.Instance.sofaForces[forceIndex].force;
                forceIndex++;
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
