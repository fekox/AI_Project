public class RotationComponent : ECSComponent
{
    public float X;
    public float Y;
    public float Z;


    public RotationComponent(float rotationX, float rotationY, float rotationZ) 
    {
        this.X = rotationX;
        this.Y = rotationY;
        this.Z = rotationZ;
    }
}
