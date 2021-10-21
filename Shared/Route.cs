using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class Route : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "date"), Display(Name = "Datum"), UIHint("Date"), Required]
        public DateTime Date { get; set; } = DateTime.Today;
        [JsonProperty(PropertyName = "author"), Display(Name = "Autor")]
        public string Author { get; set; }
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Titel", Prompt = "Titel für Link eingeben"), MaxLength(120, ErrorMessage = "Titel zu lang."), Required(ErrorMessage = "Bitte Titel eingeben.")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "shortTitle", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Kurztitel"), MaxLength(80, ErrorMessage = "Kurztitel zu lang.")]
        public string ShortTitle { get; set; }
        [JsonProperty(PropertyName = "urlTitle", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Titel-Link", Prompt = "Kurztitel wie er in der Url auftaucht"), MaxLength(80, ErrorMessage = "UrlTitel zu lang.")]
        [RegularExpression("[a-z0-9-_]*", ErrorMessage = "Bitte nur Kleinbuchstaben und Zahlen für den Titel-Link eingeben.")]
        public string UrlTitle { get; set; }
        [JsonProperty(PropertyName = "length", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Länge und Höhenmeter"), MaxLength(80, ErrorMessage = "Längenangabe zu lang.")]
        public string Length { get; set; }
        [JsonProperty(PropertyName = "description"), Display(Name = "Beschreibung"), MaxLength(5000, ErrorMessage = "Die Beschreibung ist zu lang.")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "photosLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Link zu Fotos"), UIHint("Url")]
        public string PhotosLink { get; set; }
        [JsonProperty(PropertyName = "videoLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Link zu einem Video", Prompt = "Link zu einem Video"), UIHint("Url")]
        public string VideoLink { get; set; }
        [JsonProperty(PropertyName = "routeLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Route", Prompt = "Link zur Strecke"), UIHint("Url")]
        public string RouteLink { get; set; }
        [JsonProperty(PropertyName = "routeLinkImage", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Image", Prompt = "Link zu einem Image"), UIHint("Url")]
        public string RouteLinkImage { get; set; }
        [JsonProperty(PropertyName = "routeLinkTitle", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Kurztitel"), MaxLength(80, ErrorMessage = "Kurztitel zu lang.")]
        public string RouteLinkTitle { get; set; }
    }
}
