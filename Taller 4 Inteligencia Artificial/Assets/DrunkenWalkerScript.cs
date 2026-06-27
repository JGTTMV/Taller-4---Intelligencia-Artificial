//Josefina Valdebenito y Angel Leyton

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BasicRoomGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public int totalRooms = 10;
    public float roomSpacing = 1.2f; //crea una leve separacion visual entre cada "room"
    
    //Limites de la pantalla visible, esto evita que se generen "rooms" fuera de la vista del jugador
    public int minScreenX = -8;  //Limites por defecto de un proyecto 2D en Unity
    public int maxScreenX = 8;
    public int minScreenY = -4;
    public int maxScreenY = 4;

    private HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

    void Start()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        GenerateFloorPlan();
        SpawnVisualRooms();

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Mapa generado en {stopwatch.ElapsedMilliseconds} ms"); //Imprime el tiempo que tomo en generar las "rooms" requeridas
    }

    void GenerateFloorPlan()
    {
        Vector2Int currentPos = Vector2Int.zero;
        roomPositions.Add(currentPos); //Genera la "room" en (0,0)

        //Camina hasta que se generen todas las "rooms" necesarias
        while (roomPositions.Count < totalRooms)
        {
            //Elije una direccion aleatoria: 0 = arriba, 1 = abajo, 2 = izquierda, 3 = derecha
            int direction = Random.Range(0, 4);
            Vector2Int newPos = currentPos;
            
            switch (direction)
            {
                case 0: newPos += Vector2Int.up; break;
                case 1: newPos += Vector2Int.down; break;
                case 2: newPos += Vector2Int.left; break;
                case 3: newPos += Vector2Int.right; break;
            }

            //Solo añade la "room" si la nueva posicion esta dentro de los limites de la pantalla
            if (IsWithinScreenBounds(newPos))
            {
                currentPos = newPos;
                //HashSet asegura que no se agreguen posiciones duplicadas
                roomPositions.Add(currentPos);
            }
        }
    }

    bool IsWithinScreenBounds(Vector2Int pos)
    {
        return pos.x >= minScreenX && pos.x <= maxScreenX &&
               pos.y >= minScreenY && pos.y <= maxScreenY;
    }

    void SpawnVisualRooms()
    {
        foreach (Vector2Int pos in roomPositions)
        {
            //Convierte las coordenadas de las grid en espacio del mundo y genera la "room" en esa posicion
            Vector3 worldPos = new Vector3(pos.x * roomSpacing, pos.y * roomSpacing, 0);
            Instantiate(roomPrefab, worldPos, Quaternion.identity, transform);
        }
    }
}