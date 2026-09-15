using System;

namespace HisEmrService.Services
{
    public class ClinicalService
    {
        // 1. HÀM NHẬP: Xử lý nhận dữ liệu khám lâm sàng từ Bác sĩ
        public string NhapKetQuaKham(string patientId, string symptoms, string icd10Code)
        {
            if (string.IsNullOrWhiteSpace(patientId) || string.IsNullOrWhiteSpace(symptoms))
                return "Cần nhập thông tin bắt buộc khi nhập dữ liệu!";

            return $"MÃ BN: {patientId} | TRIỆU CHỨNG: {symptoms} | MÃ ICD-10: {icd10Code.ToUpper()} | NGÀY NHẬP: {DateTime.Now:dd/MM/yyyy HH:mm}";
        }

        // 2. HÀM IN: Tiếp nhận chuỗi bản ghi đã nhập và định dạng thành lệnh in nhiệt
        public string InPhieuKetQuaKham(string rawMedicalRecord)
        {
            if (string.IsNullOrWhiteSpace(rawMedicalRecord))
                return "Cần nhập Không có dữ liệu để in!";

            return $@"
==================================================
           PHIẾU KẾT QUẢ KHÁM LÂM SÀNG
==================================================
{rawMedicalRecord}
==================================================
     Hệ thống HIS - Vui lòng lưu hồ sơ cẩn thận
================================================== HEALTH";
        }
    }
}
