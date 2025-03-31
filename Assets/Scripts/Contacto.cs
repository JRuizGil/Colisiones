using UnityEngine;
using UnityEngine.Assertions.Must;

public struct Contacto
{
    public Vector2 mPuntoContacto;      // Punto donde ocurre la colisión
    public Vector2 mDirecciónContacto;  // Dirección de la colisión
    public float mMagnitudContacto;     // Fuerza del impacto

    public Contacto(Vector2 punto, Vector2 direccion, float magnitud)
    {
        this.mPuntoContacto = punto.normalized;
        this.mDirecciónContacto = direccion.normalized; 
        this.mMagnitudContacto = magnitud;
    }
}

