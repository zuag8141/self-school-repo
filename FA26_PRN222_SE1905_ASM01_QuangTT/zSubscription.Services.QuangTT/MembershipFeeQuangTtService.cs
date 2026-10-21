using zSubscription.Entities.QuangTT.Models;

namespace zSubscription.Services.QuangTT
{
    public class MembershipFeeQuangTtService : IMembershipFeeQuangTtService
    {
        private readonly MembershipFeeQuangTtService _repository;
        public MembershipFeeQuangTtService() => _repository ??= new MembershipFeeQuangTtService();
        public async Task<int> CreateAsync(MembershipFeeQuangTt membershipFee)
        {
            try 
            {
                return await Task.Run(() => _repository.CreateAsync(membershipFee));
            } 
            catch (Exception ex) { throw new ApplicationException("Error creating Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try 
            {
                var item = await _repository.GetByIdAsync(id);
            } 
            catch (Exception ex) { throw new ApplicationException("Error deleting Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<List<MembershipFeeQuangTt>> GetAllAsync()
        {
            try
            {
                return await Task.Run(() => _repository.GetAllAsync());
            }
            catch (Exception ex) { throw new ApplicationException("Error retrieving Membership fee list " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<MembershipFeeQuangTt> GetByIdAsync(int id)
        {
            try
            {
                return await Task.Run(() => _repository.GetByIdAsync(id));
            }
            catch (Exception ex) { throw new ApplicationException("Error retrieving Membership fee by ID " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public async Task<List<MembershipFeeQuangTt>> SearchAsync(string transCode, decimal amount, string payMethod)
        {
            try 
            {
                return await Task.Run(() => _repository.SearchAsync(transCode, amount, payMethod));
            } 
            catch (Exception ex) { throw new ApplicationException("Error searching Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(MembershipFeeQuangTt membershipFee)
        {
            try 
            {
                return Task.Run(() => _repository.UpdateAsync(membershipFee));
            } 
            catch (Exception ex) { throw new ApplicationException("Error updating Membership fee " + ex.Message, ex); }
            //throw new NotImplementedException();
        }
    }
}
