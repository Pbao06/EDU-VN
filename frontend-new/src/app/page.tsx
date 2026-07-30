import { redirect } from 'next/navigation';

export default function Home() {
  // Muốn mặc định vào trang login hay trang chủ chính thì điền vào đây
  redirect('/login'); 
}