using SB.Gobernanza.Dominio.Interfaces;
using SB.Gobernanza.Dominio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aplicacion.Services
{
    public class EntityService
    {
        private readonly IFileReader _fileReader;

        public EntityService(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public async Task<IEnumerable<Entidad>> GetAllEntitiesAsync(string filePath)
        {
            return await Task.Run(() => _fileReader.ReadEntitiesFromFile(filePath));
        }

        public async Task AddEntityAsync(Entidad newEntity, string filePath)
        {
            var entities = await GetAllEntitiesAsync(filePath);
            newEntity.Id = entities.Any() ? entities.Max(e => e.Id) + 1 : 1;
            await SaveEntityToFile(newEntity, filePath);
        }

        public async Task<Entidad> GetEntityByIdAsync(int id, string filePath)
        {
            var entities = await GetAllEntitiesAsync(filePath);
            return entities.FirstOrDefault(e => e.Id == id);
        }

        public async Task UpdateEntityAsync(Entidad updatedEntity, string filePath)
        {
            var entities = await GetAllEntitiesAsync(filePath);
            var entityList = entities.ToList();
            var existingEntity = entityList.FirstOrDefault(e => e.Id == updatedEntity.Id);
            if (existingEntity != null)
            {
                existingEntity.Nombre = updatedEntity.Nombre;
                existingEntity.Tipo = updatedEntity.Tipo;
                existingEntity.Direccion = updatedEntity.Direccion;
                existingEntity.Ciudad = updatedEntity.Ciudad;
                existingEntity.Telefono = updatedEntity.Telefono;
                existingEntity.CorreoElectronico = updatedEntity.CorreoElectronico;
                await SaveEntitiesToFile(entityList, filePath);
            }
            else
            {
                throw new KeyNotFoundException("Entidad no encontrada.");
            }
        }

        public async Task DeleteEntityAsync(int id, string filePath)
        {
            var entities = await GetAllEntitiesAsync(filePath);
            var entityList = entities.ToList();
            var entityToDelete = entityList.FirstOrDefault(e => e.Id == id);
            if (entityToDelete != null)
            {
                entityList.Remove(entityToDelete);
                await SaveEntitiesToFile(entityList, filePath);
            }
            else
            {
                throw new KeyNotFoundException("Entidad no encontrada.");
            }
        }

        private async Task SaveEntityToFile(Entidad entity, string filePath)
        {
            using (var writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync($"{entity.Id},{entity.Nombre},{entity.Tipo},{entity.Direccion},{entity.Ciudad},{entity.Telefono},{entity.CorreoElectronico}");
            }
        }

        private async Task SaveEntitiesToFile(List<Entidad> entities, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var entity in entities)
                {
                    await writer.WriteLineAsync($"{entity.Id},{entity.Nombre},{entity.Tipo},{entity.Direccion},{entity.Ciudad},{entity.Telefono},{entity.CorreoElectronico}");
                }
            }
        }
    }
}
