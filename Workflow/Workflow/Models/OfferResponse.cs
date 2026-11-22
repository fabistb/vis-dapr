using System.Text.Json.Serialization;

namespace Workflow.Models;

// Required so a body without "accepted" is a 400, not a silent decline.
public record OfferResponse([property: JsonRequired] bool Accepted);
