using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kang
{
    public class AgentManager : SingleTon<AgentManager>
    {
        public GameObject agent;
        public Transform startPos;
        public float moveForce;
        public float torque;
        [Space()]
        public float power = 0.1f;
        public float ratio = 0.8f;
        public int agentCount = 10;
        public int epCount = 10;
        public float maxLifetime = 10f;
        public float vecARatio = 0.1f;
        public float vecNRatio = 0.05f;

        public float bsetScore = 0f;
        
        public Mesh bestMesh;
        public Vector2[] bestVectors;
        private PolygonCollider2D startCollider;

        List<AgentController> agents;
        List<AgentController> goalAgents;

        bool playing = false;

        private void Awake()
        {
            agents = new List<AgentController>();
            goalAgents = new List<AgentController>();
            startCollider = GetComponent<PolygonCollider2D>();
            bestVectors = startCollider.GetPath(0);
            bestMesh = GetMesh(startCollider, transform, true);
        }
        private void Start()
        {
            for (int j = 0; j < agentCount; j++)
            {
                agents.Add(Instantiate(agent, startPos.position, Quaternion.identity).GetComponent<AgentController>());
            }
        }
        public Mesh GetMesh(PolygonCollider2D col, Transform trm, bool offset = false)
        {
            Mesh mesh = col.CreateMesh(false, false);
            if(mesh == null)
            {
                Destroy(col.gameObject);
                return null;
            }
            List<Vector3> verts = new List<Vector3>();
            mesh.GetVertices(verts);

            float ratio = 1f / trm.localScale.x;

            for (int i = 0; i < verts.Count; i++)
            {
                if(offset)
                    verts[i] -= transform.position;
                verts[i] *= ratio;
            }
            mesh.SetVertices(verts);
            return mesh;
        }
        public void StartEpisode()
        {
            playing = true;
            StartCoroutine(Episode());
        }

        private IEnumerator Episode()
        {
            for (int x = 0; x < epCount; x++)
            {
                for (int j = 0; j < agentCount; j++)
                {
                    agents[j].gameObject.SetActive(true);
                    agents[j].Init();
                }
                yield return new WaitForSeconds(maxLifetime);

                for (int i = 0; i < goalAgents.Count; i++)
                {
                    if (bsetScore < goalAgents[i].score)
                    {
                        bsetScore = goalAgents[i].score;
                        bestMesh = goalAgents[i].meshFilter.mesh;
                        bestVectors = goalAgents[i].col.GetPath(0);
                    }
                }
                agents.AddRange(goalAgents);
                goalAgents = new List<AgentController>();
            }
            playing = false;
        }

        public void Goal(AgentController agentController)
        {
            goalAgents.Add(agentController);
            agents.Remove(agentController);
        }
    }
}
