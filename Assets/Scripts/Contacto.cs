using UnityEngine;

public struct Contacto
{
    public Vector2 mPuntoContacto;      // Punto donde ocurre la colisión
    public Vector2 mDirecciónContacto;  // Dirección de la colisión
    public float mMagnitudContacto;     // Fuerza del impacto

    public Contacto(Vector2 punto, Vector2 direccion, float magnitud)
    {
        mPuntoContacto = punto;
        mDirecciónContacto = direccion.normalized; // Normalizamos la dirección
        mMagnitudContacto = magnitud;
    }

    public override string ToString()
    {
        return $"Punto: {mPuntoContacto}, Dirección: {mDirecciónContacto}, Magnitud: {mMagnitudContacto}";
    }
}

