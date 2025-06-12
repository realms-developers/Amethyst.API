using Amethyst.Systems.Characters.Base;
using Amethyst.Systems.Characters.Base.Factories;
using Amethyst.Systems.Characters.Base.Interactions;
using Amethyst.Systems.Characters.Clientside.Factories.Interactions;
using Amethyst.Systems.Characters.Storages.MongoDB;
using Amethyst.Systems.Users.Base;
using Amethyst.Systems.Users.Players;

namespace Amethyst.Systems.Characters.Clientside.Factories;

public sealed class ClientsideCharacterFactory : ICharacterFactory<ClientsideCharacterProvider>
{
    static ClientsideCharacterFactory()
    {
        MongoCharacterModel.Storage ??= new MongoCharactersStorage();
    }

    public IInteractionFactory<ICharacterEditor> EditorFactory { get; set; } = new ClientsideEditorFactory();

    public IInteractionFactory<ICharacterHandler> HandlerFactory { get; set; } = new ClientsideHandlerFactory();

    public IInteractionFactory<ICharacterSynchroniser> SynchronizerFactory { get; set; } = new ClientsideSynchroniserFactory();

    public ICharactersStorage Storage { get; set; } = new MongoCharactersStorage();

    public IDefaultModelFactory ModelFactory { get; set; } = new ClientsideModelFactory();

    public ClientsideCharacterProvider BuildFor(IAmethystUser user)
    {
        if (user is not PlayerUser plrUser)
        {
            throw new ArgumentException("User is not a PlayerUser", nameof(user));
        }

        ICharacterModel model = ModelFactory.CreateModel(plrUser.Player);
        ClientsideCharacterProvider provider = CreateProvider(plrUser);

        provider.LoadModel(model);

        return provider;
    }

    private ClientsideCharacterProvider CreateProvider(PlayerUser user)
    {
        var provider = new ClientsideCharacterProvider(user);

        provider.Editor = EditorFactory.BuildFor(provider);
        provider.Handler = HandlerFactory.BuildFor(provider);
        provider.Synchronizer = SynchronizerFactory.BuildFor(provider);

        return provider;
    }
}
