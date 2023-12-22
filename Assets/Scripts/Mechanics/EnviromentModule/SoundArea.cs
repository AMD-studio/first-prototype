using UnityEngine;


namespace Enviroment
{
    internal class SoundArea : MonoBehaviour
    {
        public float soundRadius = 10f; // ������ �����
        public float fadeDistance = 20f; // ����������, �� ������� ���� �������
        public int numberOfPoints = 10; // ���������� ����� � ����
        public Transform soundPointPrefab; // ������ ��� ����� ��������� ���������

        private void Start()
        {
            CreateSoundPoints();
        }

        private void CreateSoundPoints()
        {
            for (int i = 0; i < numberOfPoints; i++)
            {
                float randomX = Random.Range(-soundRadius, soundRadius);
                float randomY = Random.Range(-soundRadius, soundRadius);
                float randomZ = Random.Range(-soundRadius, soundRadius);

                Vector3 soundPointPosition = transform.position + new Vector3(randomX, randomY, randomZ);
                Transform soundPoint = Instantiate(soundPointPrefab, soundPointPosition, Quaternion.identity);
                soundPoint.SetParent(transform);

                // ������������ �������� ������ � ��������
                float distance = Vector3.Distance(transform.position, soundPointPosition);
                float volume = Mathf.Clamp01(1 - distance / fadeDistance);

                // ������������� ��������� ����� �����
                AudioSource audioSource = soundPoint.GetComponent<AudioSource>();
                audioSource.volume = volume;
            }
        }
    }
}