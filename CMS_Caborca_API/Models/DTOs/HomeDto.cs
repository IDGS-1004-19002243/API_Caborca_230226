namespace CMS_Caborca_API.Models.DTOs
{
    public class HomeDto
    {
        // Carousel Principal
        public List<HomeCarouselItemDto> Carousel { get; set; } = new();

        // Sección "¿Quieres ser distribuidor?" (formulario)
        public HomeSeccionDto FormDistribuidor { get; set; } = new();

        // Sección "Sustentabilidad" (banner izquierdo)
        public HomeSeccionDto Sustentabilidad { get; set; } = new();

        // Sección "Arte de la Creación" (Nosotros)
        public HomeArteCreacionDto ArteCreacion { get; set; } = new();

        // Sección "Distribuidores Autorizados" (logos)
        public HomeDistribuidoresLogosDto DistribuidoresLogos { get; set; } = new();
    }

    // ─── Carousel ───────────────────────────────────────────────────────────────
    public class HomeCarouselItemDto
    {
        public int Id { get; set; }
        public string Titulo_ES { get; set; } = string.Empty;
        public string Titulo_EN { get; set; } = string.Empty;
        public string Subtitulo_ES { get; set; } = string.Empty;
        public string Subtitulo_EN { get; set; } = string.Empty;
        public string TextoBoton_ES { get; set; } = string.Empty;
        public string TextoBoton_EN { get; set; } = string.Empty;
        public string LinkBoton { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public int Orden { get; set; }
    }

    // ─── Sección genérica ampliada ───────────────────────────────────────────────
    public class HomeSeccionDto
    {
        public string Titulo_ES { get; set; } = string.Empty;
        public string Titulo_EN { get; set; } = string.Empty;
        public string Descripcion_ES { get; set; } = string.Empty;
        public string Descripcion_EN { get; set; } = string.Empty;
        public string TextoBoton_ES { get; set; } = string.Empty;
        public string TextoBoton_EN { get; set; } = string.Empty;
        public string LinkBoton { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        // Campos extras para Sustentabilidad
        public string Badge_ES { get; set; } = string.Empty;
        public string Badge_EN { get; set; } = string.Empty;
        public string TituloDerecho_ES { get; set; } = string.Empty;
        public string TituloDerecho_EN { get; set; } = string.Empty;
        public string NotaCertificacion_ES { get; set; } = string.Empty;
        public string NotaCertificacion_EN { get; set; } = string.Empty;
        // Campos extras para FormDistribuidor
        public string NotaTiempo_ES { get; set; } = string.Empty;
        public string NotaTiempo_EN { get; set; } = string.Empty;
        public string StatDistribuidores { get; set; } = string.Empty;
        public string StatEstados { get; set; } = string.Empty;
    }

    // ─── Arte de la Creación ────────────────────────────────────────────────────
    public class HomeArteCreacionDto
    {
        public string Badge_ES { get; set; } = string.Empty;
        public string Badge_EN { get; set; } = string.Empty;
        public string Titulo_ES { get; set; } = string.Empty;
        public string Titulo_EN { get; set; } = string.Empty;
        public int AnosExperiencia { get; set; } = 40;
        public string ImagenUrl { get; set; } = string.Empty;
        public string Boton_ES { get; set; } = string.Empty;
        public string Boton_EN { get; set; } = string.Empty;
        public string Nota_ES { get; set; } = string.Empty;
        public string Nota_EN { get; set; } = string.Empty;
        public List<HomeFeatureDto> Features { get; set; } = new();
    }

    public class HomeFeatureDto
    {
        public string Titulo_ES { get; set; } = string.Empty;
        public string Titulo_EN { get; set; } = string.Empty;
        public string Descripcion_ES { get; set; } = string.Empty;
        public string Descripcion_EN { get; set; } = string.Empty;
    }

    // ─── Distribuidores Logos ───────────────────────────────────────────────────
    public class HomeDistribuidoresLogosDto
    {
        public string Titulo_ES { get; set; } = string.Empty;
        public string Titulo_EN { get; set; } = string.Empty;
        public string Subtitulo_ES { get; set; } = string.Empty;
        public string Subtitulo_EN { get; set; } = string.Empty;
        public string TextoBoton_ES { get; set; } = string.Empty;
        public string TextoBoton_EN { get; set; } = string.Empty;
        public List<HomeDistribuidorLogoItemDto> Logos { get; set; } = new();
    }

    public class HomeDistribuidorLogoItemDto
    {
        public int Id { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }
}