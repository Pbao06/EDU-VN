'use client';

import { useRouter } from 'next/navigation';
import CareerQuizCard, { type CareerQuizFormData } from "@/components/shared/OnBoarding";
import { useOnboarding } from "@/hooks/auth/useOnboarding"; // Import hook

const CareerQuizCardPage = () => {
    const router = useRouter();
    // 1. Gọi hook để lấy hàm xử lý và các trạng thái loading, error
    const { completeOnboarding, loading, error } = useOnboarding();

    // 2. Hàm handleSubmit giờ sẽ gọi hàm `completeOnboarding` từ hook
    const handleSubmit = async (data: CareerQuizFormData) => {
        try {
            console.log("Dữ liệu gửi đi:", data);
            
            // 3. Gọi hàm `completeOnboarding` từ hook
            const response = await completeOnboarding(data);
            
            console.log("API trả về:", response);// debug
            
            // Nếu thành công, có thể chuyển người dùng sang trang khác
            //alert("Gửi thông tin thành công!");
            router.push('/');

        } catch (err: any) {
            // Lỗi đã được xử lý bên trong hook và được lưu vào state `error` của hook
            console.error("Lỗi khi gọi API:", err);
            // `error` ở đây là giá trị từ hook, nó sẽ tự động cập nhật nếu có lỗi.
            // Chúng ta không cần gán lại error ở đây.
            alert(`Có lỗi xảy ra: ${error || "Vui lòng thử lại."}`); 
        }
    };

    return (
        <>
            <CareerQuizCard
                title="Khám phá nghề nghiệp"
                description="Hãy để chúng tôi giúp bạn tìm ra con đường phù hợp."
                onSubmit={handleSubmit}
                loading={loading} // 4. Truyền trạng thái loading từ hook xuống component con
            />
            {/* Bạn có thể hiển thị lỗi từ hook ở đây nếu muốn */}
            {error && (
                <div style={{ color: 'red', textAlign: 'center', padding: '10px' }}>
                    Lỗi: {error}
                </div>
            )}
        </>
    );
};
export default CareerQuizCardPage;