public class ParagonConfig<T>
{
    public T Normal { get; set; }
    public T Paragon { get; set; }

    public ParagonConfig() {}
    public ParagonConfig(T defaultValue) : this(defaultValue, defaultValue)
    {
    }

    public ParagonConfig(T normal, T paragon)
    {
        Normal = normal;
        Paragon = paragon;
    }
}