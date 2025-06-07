using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Serverside.Factories.Interactions;
using Amethyst.Systems.Characters.Storages.MongoDB;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Systems.Characters.Serverside.Factories;

public sealed class ServersideCharacterFactory : ICharacterFactory<ServersideCharacterProvider>
{
    static ServersideCharacterFactory()
    {
        MongoCharacterModel.Storage ??= new MongoCharactersStorage();
    }

    public IInteractionFactory<ICharacterEditor> EditorFactory { get; set; } = new ServersideEditorFactory();

    public IInteractionFactory<ICharacterHandler> HandlerFactory { get; set; } = new ServersideHandlerFactory();

    public IInteractionFactory<ICharacterSynchroniser> SynchronizerFactory { get; set; } = new ServersideSynchroniserFactory();

    public ICharactersStorage Storage { get; set; } = new MongoCharactersStorage();

    public IDefaultModelFactory ModelFactory { get; set; } = new ConfigModelFactory();

    public ServersideCharacterProvider BuildFor(IAmethystUser user)
    {
        if (user is not PlayerUser plrUser)
            throw new ArgumentException("User is not a PlayerUser", nameof(user));

        var model = FindOrCreateModel(plrUser);
        var provider = CreateProvider(plrUser);

        provider.LoadModel(model);

        return provider;
    }

    private ServersideCharacterProvider CreateProvider(PlayerUser user)
    {
        var provider = new ServersideCharacterProvider(user);

        provider.Editor = EditorFactory.BuildFor(provider);
        provider.Handler = HandlerFactory.BuildFor(provider);
        provider.Synchronizer = SynchronizerFactory.BuildFor(provider);

        return provider;
    }

    private ICharacterModel FindOrCreateModel(PlayerUser user)
    {
        var model = Storage.GetModel(user.Name);

        if (model is null)
        {
            model = Storage.Convert(ModelFactory.CreateModel(user.Player));
            Storage.SaveModel(model);
        }

        return model;
    }
}
