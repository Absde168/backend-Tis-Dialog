namespace deaplom.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Хеширует пароль с помощью BCrypt (cost factor = 11)
        /// </summary>
        public static string Hash(string password)
            => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

        /// <summary>
        /// Проверяет пароль против сохранённого BCrypt-хеша
        /// </summary>
        public static bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
