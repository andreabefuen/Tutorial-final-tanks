using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for(int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigibody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigibody)
                continue;

            targetRigibody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth targetHealth = targetRigibody.GetComponent<TankHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigibody.position);

            targetHealth.TakeDamage(damage);
        }


        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        Destroy(m_ExplosionParticles.gameObject, t: m_ExplosionParticles.duration);
        Destroy(gameObject); //destruye la bala
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

        Vector3 explotionToTarget = targetPosition - transform.position;

        float explotionDistance = explotionToTarget.magnitude;

        //Muy grande cuando está cerca y muy pequeño cuando está lejos
        float relativeDistance = (m_ExplosionRadius - explotionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage; //Relative distance es un valor entre 00 y 1
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}