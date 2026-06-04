namespace Drajbot.Api.Utils
{
    public static class EmailTemplates
    {
        // Zajednički okvir za sve mejlove (Dizajn, boje, footer)
        public static string GetBaseTemplate(string title, string content)
        {
            return $@"
                <div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #0d1117; color: #ffffff; padding: 40px 20px; text-align: center;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #161b22; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.5);'>
                    
                    <div style='background-color: #161b22; padding: 20px 0 0 0; text-align: center;'>
                        <img src='https://i.ibb.co/SwPjmv3H/Gemini-Generated-Image-twtufdtwtufdtwtu-removebg-preview.png' alt='Drajbot Logo' style='max-height: 60px; object-fit: contain;' />
                    </div>

                    <div style='background-color: #161b22; padding: 10px 20px 20px 20px; font-size: 24px; font-weight: bold; letter-spacing: 2px; color: #00BCD4;'>
                        D'RAJBOT GAME SHOP
                    </div>
                    <div style='padding: 40px 30px; text-align: left; font-size: 16px; line-height: 1.6; color: #c9d1d9;'>
                        <h2 style='color: #ffffff; margin-top: 0;'>{title}</h2>
                        {content}
                    </div>
                    <div style='background-color: #0d1117; padding: 20px; font-size: 12px; color: #8b949e; border-top: 1px solid #30363d;'>
                        <p>Ovo je automatska poruka. <strong>Molimo vas da ne odgovarate na ovaj email.</strong></p>
                        <p>Ukoliko imate problema, kontaktirajte našu podršku na sajtu.</p>
                        <p><a href='#' style='color: #00BCD4; text-decoration: none;'>Pravila korišćenja</a> | <a href='#' style='color: #00BCD4; text-decoration: none;'>Politika privatnosti</a></p>
                        <p>&copy; {DateTime.UtcNow.Year} D'RAJBOT. Sva prava zadržana.</p>
                    </div>
                </div>
            </div>";
        }

        public static string GetWelcomeEmail(string firstName, string username)
        {
            string content = $@"
                <p>Zdravo <strong>{firstName}</strong>,</p>
                <p>Dobrodošli u D'RAJBOT Game Shop! Vaš nalog je uspešno kreiran.</p>
                <p>Vaše korisničko ime je: <strong style='color: #00BCD4;'>{username}</strong></p>
                <p>Sada možete da pregledate našu ponudu, dodajete igre u listu želja i kupujete dopune za igrice po najpovoljnijim cenama.</p>
                <p>Srećno gejmanje!</p>";

            return GetBaseTemplate("Dobrodošli!", content);
        }

        public static string GetForgotPasswordEmail(string token)
        {
            string content = $@"
                <p>Primili smo zahtev za resetovanje lozinke na vašem nalogu.</p>
                <p>Vaš sigurnosni kod za resetovanje je:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='background-color: #21262d; border: 2px dashed #00BCD4; padding: 15px 30px; font-size: 32px; font-weight: bold; letter-spacing: 5px; border-radius: 8px;'>{token}</span>
                </div>
                <p style='color: #ff7b72;'>Ovaj kod ističe za 15 minuta. Ne delite ga ni sa kim!</p>
                <p>Ako niste vi zatražili resetovanje lozinke, možete bezbedno ignorisati ovaj email.</p>";

            return GetBaseTemplate("Resetovanje lozinke", content);
        }

        public static string GetPasswordChangedEmail()
        {
            string content = $@"
                <p>Vaša lozinka je upravo <strong>uspešno promenjena</strong>.</p>
                <p>Sada možete da se prijavite na svoj nalog koristeći novu lozinku.</p>
                <p>Ako vi niste izvršili ovu promenu, hitno nas kontaktirajte!</p>";

            return GetBaseTemplate("Lozinka je promenjena", content);
        }
    }
}