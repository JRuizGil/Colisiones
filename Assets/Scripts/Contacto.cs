using UnityEngine;

public struct Contacto
{
    public Vector2 mPuntoContacto;      // Punto donde ocurre la colisi�n
    public Vector2 mDirecci�nContacto;  // Direcci�n de la colisi�n
    public float mMagnitudContacto;     // Fuerza del impacto

    public Contacto(Vector2 punto, Vector2 direccion, float magnitud)
    {
        mPuntoContacto = punto;
        mDirecci�nContacto = direccion.normalized; // Normalizamos la direcci�n
        mMagnitudContacto = magnitud;
    }

    public override string ToString()
    {
        return $"Punto: {mPuntoContacto}, Direcci�n: {mDirecci�nContacto}, Magnitud: {mMagnitudContacto}";
    }
}

