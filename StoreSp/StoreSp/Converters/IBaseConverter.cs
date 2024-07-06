namespace StoreSp.Converters;

public interface IBaseConverter<Entity,Dto>
{
    public Dto ToDto(Entity entity);
    public Entity ToEntity(Dto dto);
}
