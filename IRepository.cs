namespace CinemaHallSystem.DAL
{
    /// <summary>
    /// Generic repository interface defining standard CRUD operations.
    /// Every DAL class implements this for its entity type.
    /// This is the ABSTRACTION talking point for your viva —
    /// "we program to an interface, not a concrete class."
    /// </summary>
    /// <typeparam name="T">The domain model type (e.g., Movie, Hall, Booking)</typeparam>
    public interface IRepository<T>
    {
        /// <summary>Inserts a new entity. Returns the generated ID.</summary>
        int Add(T entity);

        /// <summary>Updates an existing entity by its ID.</summary>
        void Update(T entity);

        /// <summary>Deletes an entity by its ID.</summary>
        void Delete(int id);

        /// <summary>Retrieves a single entity by its primary key.</summary>
        T GetById(int id);

        /// <summary>Retrieves all entities of this type.</summary>
        List<T> GetAll();
    }
}
