

using Optern.Application.DTOs.Skills;
using Optern.Application.Interfaces.ISkillService;

[ExtendObjectType("Query")]
public class SkillQuery
     {
    [GraphQLDescription("Get Suggestions Skills")]
    public async Task<Response<List<string>>>GetSkillSuggestions([Service] ISkillService _skillService, string word)=>
        await _skillService.GetSkillSuggestions(word);

     }