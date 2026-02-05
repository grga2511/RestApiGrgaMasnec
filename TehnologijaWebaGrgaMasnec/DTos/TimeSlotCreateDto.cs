using System.Text.Json.Serialization;

namespace TehnologijaWebaGrgaMasnec.DTos;

public record TimeSlotCreateDto(
    int RoomId,
    [property: JsonPropertyName("start")] DateTime Start,
    [property: JsonPropertyName("end")] DateTime End
);
