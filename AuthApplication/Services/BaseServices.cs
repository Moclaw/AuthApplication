using AuthApplication.Models;
using AuthApplication.Models.Entity;

namespace AuthApplication.Services
{
    public class BaseServices<T> where T : BaseEntity
    {
        public readonly ApplicationDbContext db;
        public readonly IConfiguration _configuration;

        public readonly ILogger<T> _logger;
        public BaseServices(ApplicationDbContext context, IConfiguration configuration, ILogger<T> logger)
        {
            db = context;
            _configuration = configuration;
            _logger = logger;
        }

		public T? GetById(int id)
		{
			try
			{
				return db?.Set<T>()?.Find(id) ?? null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
			finally
			{
				db.Dispose();
			}
		}

		public List<T>? GetByIds(List<int> ids)
		{
			try
			{
                return db?.Set<T>()?.Where(c => ids.Contains(c.Id))?.ToList() ?? null;
            }
            catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
			finally
			{
				db.Dispose();
			}
		}

		public List<T>? GetAll()
		{
			try
			{
				return db?.Set<T>()?.ToList() ?? null;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
			finally
			{
				db.Dispose();
			}
		}

        public bool Insert(T entity)
        {
            try
            {
                db.Add(entity);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            finally
            {
                db.Dispose();
            }
        }

		public bool Insert(List<T> entities)
		{
			try
			{
				db.AddRange(entities);
				db.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}

		public async Task<bool> InsertAsync(T entity)
		{
			try
			{
				await db.AddAsync(entity);
				await db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}

		public async Task<bool> InsertAsync(List<T> entities)
		{
			try
			{
				await db.AddRangeAsync(entities);
				await db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}

		public bool Update(T entity)
		{
			try
			{
				db.Update(entity);
				db.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}

		public bool Update(List<T> entities)
		{
			try
			{
				db.UpdateRange(entities);
				db.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}

		public async Task<bool> UpdateAsync(T entity)
		{
			try
			{
				db.Update(entity);
				await db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}

		public async Task<bool> UpdateAsync(List<T> entities)
		{
			try
			{
				db.UpdateRange(entities);
				await db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
			finally
			{
				db.Dispose();
			}
		}
    }
}
