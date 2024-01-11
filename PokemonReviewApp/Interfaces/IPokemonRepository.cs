// IPokemonRepository.cs
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    // Define an interface for Pokemon repository
    public interface IPokemonRepository
    {
        // Method to get a collection of Pokemon
        ICollection<Pokemon> GetPokemons();

        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        decimal GetPokemonRating(int pokeId);

        bool PokemonExists(int pokeId);

        bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemonId);
        bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemonId);
        bool DeletePokemon(Pokemon pokemon);
        bool Save();


    }
}