// PokemonController.cs
using PokemonReviewApp.Models;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Interfaces;
using AutoMapper;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    // Define a controller for handling Pokemon-related requests
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository reviewRepository;
        private readonly IMapper _mapper;

        // Constructor to inject an instance of IPokemonRepository
        public PokemonController(IPokemonRepository pokemonRepository,
            IReviewRepository reviewRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            this.reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        // Define an HTTP GET endpoint to get a list of Pokemon
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            // Call the GetPokemons method from the injected repository
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            // Check if the model state is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Return the list of Pokemon as a 200 OK response
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200,Type=typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId) 
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var pokemon=_mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId) 
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
            {
                return BadRequest(ModelState);
            }

            var pokemons = _pokemonRepository.GetPokemons()
                .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);



            if (!_pokemonRepository.CreatePokemon(ownerId,catId,pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId, 
            [FromQuery] int ownerId, 
            [FromQuery] int catId, 
            [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId,catId,pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var reviewsToDelete = reviewRepository.GetReviewsOfAPokemon(pokeId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong");
            }

            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
            }

            return NoContent();
        }
    }
}