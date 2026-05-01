namespace LIMTIC.E2Es
{
    [CollectionDefinition("E2E collection")]
    public class E2ECollection : ICollectionFixture<PostgresFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
