namespace eVotingSystemWebAPIsBackUp.DTOs
{
    public class ElectionResultsResponseDto
    {
        public ElectionResultDto National { get; set; }
        public ElectionResultDto Provincial { get; set; }
        public ElectionResultDto Regional { get; set; }
    }
}
