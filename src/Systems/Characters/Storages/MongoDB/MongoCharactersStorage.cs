using System.Data.Entity;
using Amethyst.Storages;
using Amethyst.Storages.Mongo;
using Amethyst.Systems.Characters.Base;

namespace Amethyst.Systems.Characters.Storages.MongoDB;

public sealed class MongoCharactersStorage : ICharactersStorage
{
    public MongoCharactersStorage()
    {
        Database = new MongoDatabase(
            CharactersConfiguration.Instance.MongoConnection ?? StorageConfiguration.Instance.MongoConnection,
            CharactersConfiguration.Instance.MongoDatabaseName ?? StorageConfiguration.Instance.MongoDatabaseName);

        Models = Database.Get<MongoCharacterModel>(
            CharactersConfiguration.Instance.MongoCollectionName);
    }

    public MongoDatabase Database { get; }

    public MongoModels<MongoCharacterModel> Models { get; }

    public Dictionary<string, ICharacterModel> ModelsCache { get; } = new();

    public ICharacterModel CreateModel(string name)
    {
        if (ModelsCache.TryGetValue(name, out ICharacterModel? model))
            return model;

        model = new MongoCharacterModel(name);

        ModelsCache.Add(name, model);

        return model;
    }

    public ICharacterModel? GetModel(string name)
    {
        if (ModelsCache.TryGetValue(name, out ICharacterModel? model))
            return model;

        model = Models.Find(name);

        if (model == null)
            return null;

        ModelsCache.Add(name, model);

        return model;
    }

    public void RemoveModel(ICharacterModel character)
    {
        ThrowIfInvalidModel(character);

        if (ModelsCache.ContainsKey(character.Name))
            ModelsCache.Remove(character.Name);

        Models.Remove(character.Name);
    }

    public void SaveModel(ICharacterModel character)
    {
        ThrowIfInvalidModel(character);

        if (!ModelsCache.TryAdd(character.Name, character))
            ModelsCache[character.Name] = character;

        Models.Save((MongoCharacterModel)character);
    }

    private void ThrowIfInvalidModel(ICharacterModel character)
    {
        if (character is not MongoCharacterModel)
            throw new InvalidOperationException("Invalid character model type.");

        if (character.Name == null)
            throw new InvalidOperationException("Character name cannot be null.");
    }
}
