using UnityEngine;
using UnityEngine.Assertions.Must;

public struct Contacto
{
    public Vector2 mPuntoContacto;      // Punto donde ocurre la colisi�n
    public Vector2 mDirecci�nContacto;  // Direcci�n de la colisi�n
    public float mMagnitudContacto;     // Fuerza del impacto

    public Contacto(Vector2 punto, Vector2 direccion, float magnitud)
    {
        this.mPuntoContacto = punto.normalized;
        this.mDirecci�nContacto = direccion.normalized; 
        this.mMagnitudContacto = magnitud;
    }
}

