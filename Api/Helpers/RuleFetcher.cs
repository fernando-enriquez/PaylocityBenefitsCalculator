
using Api.Interfaces;
using Api.Models;

namespace Api.Helpers
{
    public static class RuleFetcher
    {
        public static async Task<Rule> GetRequiredRuleAsync(IRepository<Rule> repository, string concept)
        {
            var rule = await repository.GetFirstOrDefaultAsync(x => x.Concept == concept);

            if (rule == null || rule.Value == null)
            {
                throw new InvalidOperationException($"Rule '{concept}' is not properly configured.");
            }

            return rule;
        }
    }
}
