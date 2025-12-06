using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using TeacherScheduleFrontend.Models;

namespace TeacherScheduleFrontend.Services
{
    public static class ApiService
    {
        private static HttpClient _httpClient = null!;
        private static string _baseUrl = "";
        private static string _token = "";

        // Thông tin user đang đăng nhập
        public static LoginResponse? CurrentUser { get; private set; }
        public static bool IsLoggedIn => !string.IsNullOrEmpty(_token);

        public static void Initialize()
        {
            // Đọc cấu hình từ appsettings.json
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    var config = JsonConvert.DeserializeObject<dynamic>(json);
                    _baseUrl = config?.ApiSettings?.BaseUrl ?? "https://teacherscheduleapi-production.up.railway.app/";
                }
                else
                {
                    _baseUrl = "https://teacherscheduleapi-production.up.railway.app/";
                }
            }
            catch
            {
                _baseUrl = "https://teacherscheduleapi-production.up.railway.app/";
            }

            // Ensure base URL always ends with a single '/'
            _baseUrl = (_baseUrl ?? string.Empty).Trim();
            if (!string.IsNullOrEmpty(_baseUrl))
                _baseUrl = _baseUrl.TrimEnd('/') + "/";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void SetToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static void ClearToken()
        {
            _token = "";
            CurrentUser = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // ==================== AUTH ====================

        public static async Task<(bool Success, string Message, LoginResponse? Data)> LoginAsync(string maTK, string matKhau)
        {
            try
            {
                var request = new LoginRequest { MaTK = maTK, MatKhau = matKhau };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/Auth/Login") { Content = content };
                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Resolve final RequestUri correctly (avoid naive concatenation)
                    Uri resolvedUri;
                    if (httpRequest.RequestUri == null)
                        resolvedUri = _httpClient.BaseAddress!;
                    else if (httpRequest.RequestUri.IsAbsoluteUri)
                        resolvedUri = httpRequest.RequestUri;
                    else
                        resolvedUri = new Uri(_httpClient.BaseAddress!, httpRequest.RequestUri);

                    var diag = new StringBuilder();
                    diag.AppendLine($"RequestUri: {resolvedUri}");
                    diag.AppendLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");
                    diag.AppendLine("Request Body:");
                    diag.AppendLine(json);
                    diag.AppendLine();
                    diag.AppendLine("Response Body:");
                    diag.AppendLine(string.IsNullOrWhiteSpace(responseContent) ? "(empty)" : responseContent);

                    MessageBox.Show(diag.ToString(), "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    if (data != null)
                    {
                        SetToken(data.Token);
                        CurrentUser = data;
                        return (true, "Đăng nhập thành công!", data);
                    }
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? $"Đăng nhập thất bại: {(int)response.StatusCode} {response.ReasonPhrase}", null);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Không thể kết nối đến server: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public static void Logout()
        {
            ClearToken();
        }

        public static async Task<(bool Success, string Message)> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Auth/ChangePassword", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Đổi mật khẩu thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Đổi mật khẩu thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== KHOA ====================

        public static async Task<List<Khoa>> GetKhoasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/Khoa");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Khoa>>(json) ?? new List<Khoa>();
                }
            }
            catch { }
            return new List<Khoa>();
        }

        public static async Task<Khoa?> GetKhoaAsync(string maKhoa)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/Khoa/{maKhoa}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Khoa>(json);
                }
            }
            catch { }
            return null;
        }

        public static async Task<(bool Success, string Message)> CreateKhoaAsync(Khoa khoa)
        {
            try
            {
                var json = JsonConvert.SerializeObject(khoa);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Khoa", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm khoa thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm khoa thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> UpdateKhoaAsync(string maKhoa, Khoa khoa)
        {
            try
            {
                var json = JsonConvert.SerializeObject(khoa);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/Khoa/{maKhoa}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật khoa thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật khoa thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteKhoaAsync(string maKhoa)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/Khoa/{maKhoa}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa khoa thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa khoa thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== BỘ MÔN ====================

        public static async Task<List<BoMon>> GetBoMonsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/BoMon");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<BoMon>>(json) ?? new List<BoMon>();
                }
            }
            catch { }
            return new List<BoMon>();
        }

        public static async Task<List<BoMon>> GetBoMonsByKhoaAsync(string maKhoa)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/BoMon/Khoa/{maKhoa}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<BoMon>>(json) ?? new List<BoMon>();
                }
            }
            catch { }
            return new List<BoMon>();
        }

        public static async Task<(bool Success, string Message)> CreateBoMonAsync(BoMon boMon)
        {
            try
            {
                var json = JsonConvert.SerializeObject(boMon);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/BoMon", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm bộ môn thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm bộ môn thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> UpdateBoMonAsync(string maBM, BoMon boMon)
        {
            try
            {
                var json = JsonConvert.SerializeObject(boMon);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/BoMon/{maBM}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật bộ môn thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật bộ môn thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteBoMonAsync(string maBM)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/BoMon/{maBM}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa bộ môn thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa bộ môn thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== GIẢNG VIÊN ====================

        public static async Task<List<GiangVien>> GetGiangViensAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/GiangVien");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<GiangVien>>(json) ?? new List<GiangVien>();
                }
            }
            catch { }
            return new List<GiangVien>();
        }

        public static async Task<GiangVien?> GetGiangVienAsync(string maGV)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/GiangVien/{maGV}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<GiangVien>(json);
                }
            }
            catch { }
            return null;
        }

        public static async Task<(bool Success, string Message)> CreateGiangVienAsync(GiangVien giangVien)
        {
            try
            {
                var json = JsonConvert.SerializeObject(giangVien);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/GiangVien", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm giảng viên thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm giảng viên thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> UpdateGiangVienAsync(string maGV, GiangVien giangVien)
        {
            try
            {
                var json = JsonConvert.SerializeObject(giangVien);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/GiangVien/{maGV}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật giảng viên thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật giảng viên thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteGiangVienAsync(string maGV)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/GiangVien/{maGV}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa giảng viên thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa giảng viên thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== MÔN HỌC ====================

        public static async Task<List<MonHoc>> GetMonHocsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/MonHoc");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<MonHoc>>(json) ?? new List<MonHoc>();
                }
            }
            catch { }
            return new List<MonHoc>();
        }

        public static async Task<(bool Success, string Message)> CreateMonHocAsync(MonHoc monHoc)
        {
            try
            {
                var json = JsonConvert.SerializeObject(monHoc);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/MonHoc", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm môn học thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm môn học thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> UpdateMonHocAsync(string maMH, MonHoc monHoc)
        {
            try
            {
                var json = JsonConvert.SerializeObject(monHoc);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/MonHoc/{maMH}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật môn học thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật môn học thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteMonHocAsync(string maMH)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/MonHoc/{maMH}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa môn học thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa môn học thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== LỚP ====================

        public static async Task<List<Lop>> GetLopsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/Lop");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Lop>>(json) ?? new List<Lop>();
                }
            }
            catch { }
            return new List<Lop>();
        }

        public static async Task<(bool Success, string Message)> CreateLopAsync(Lop lop)
        {
            try
            {
                var json = JsonConvert.SerializeObject(lop);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Lop", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm lớp thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm lớp thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> UpdateLopAsync(string maLop, Lop lop)
        {
            try
            {
                var json = JsonConvert.SerializeObject(lop);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/Lop/{maLop}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật lớp thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật lớp thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteLopAsync(string maLop)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/Lop/{maLop}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa lớp thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa lớp thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== PHÂN CÔNG ====================

        public static async Task<List<PhanCong>> GetPhanCongsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/PhanCong");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<PhanCong>>(json) ?? new List<PhanCong>();
                }
            }
            catch { }
            return new List<PhanCong>();
        }

        public static async Task<(bool Success, string Message)> CreatePhanCongAsync(PhanCong phanCong)
        {
            try
            {
                var json = JsonConvert.SerializeObject(phanCong);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/PhanCong", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm phân công thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm phân công thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> UpdatePhanCongAsync(int id, PhanCong phanCong)
        {
            try
            {
                var json = JsonConvert.SerializeObject(phanCong);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/PhanCong/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật phân công thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật phân công thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeletePhanCongAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/PhanCong/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa phân công thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa phân công thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        // ==================== ĐỊNH MỨC ====================

        public static async Task<List<DinhMuc>> GetDinhMucsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/DinhMuc");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<DinhMuc>>(json) ?? new List<DinhMuc>();
                }
            }
            catch { }
            return new List<DinhMuc>();
        }

        public static async Task<(bool Success, string Message)> CreateDinhMucAsync(DinhMuc dinhMuc)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dinhMuc);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/DinhMuc", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Thêm định mức thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Thêm định mức thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        public static async Task<(bool Success, string Message)> UpdateDinhMucAsync(int id, DinhMuc dinhMuc)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dinhMuc);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/DinhMuc/{id}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật định mức thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Cập nhật định mức thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteDinhMucAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/DinhMuc/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa định mức thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa định mức thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        // ==================== TÀI KHOẢN ====================

        public static async Task<List<TaiKhoan>> GetTaiKhoansAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/TaiKhoan");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<TaiKhoan>>(json) ?? new List<TaiKhoan>();
                }
            }
            catch { }
            return new List<TaiKhoan>();
        }

        public static async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Auth/Register", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Đăng ký tài khoản thành công!");
                }

                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Đăng ký thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> DeleteTaiKhoanAsync(string maTK)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/TaiKhoan/{maTK}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa tài khoản thành công!");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                return (false, error?.Message ?? "Xóa tài khoản thất bại!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
    }
}
