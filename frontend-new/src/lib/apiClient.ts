const BASE_URL = 'http://localhost:5000';

type ApiClientOptions = Omit<RequestInit, 'body'> & {
  headers?: Record<string, string>;
  body?: unknown;
};

async function request<T>(path: string, options: ApiClientOptions = {}): Promise<T> {
  const { body, headers, ...rest } = options;

  const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null;

  const config: RequestInit = {
    ...rest,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
    body: body ? JSON.stringify(body) : undefined,
  };

  try {
    const response = await fetch(`${BASE_URL}${path}`, config);

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || response.statusText);
    }

    const text = await response.text();
    if (!text) throw new Error(" Server returned no contend");

    const json = JSON.parse(text);

    // TỰ ĐỘNG BÓC TÁCH: Nếu backend trả về { data: T, message: string }
    // thì trả về T. Nếu không phải cấu trúc đó thì trả về nguyên json.
    return (json && typeof json === 'object' && 'data' in json) ? json.data : json;
  } catch (error) {
    console.error(`API Request failed for ${path}:`, error);
    // 👉 Nếu lỗi đã được ném chủ động từ bên trên (ví dụ: errorText từ server hoặc message của chúng ta)
    if (error instanceof Error) {
      throw error;
    }
    throw new Error(`Failed to connect to backend server at ${BASE_URL}. Please ensure the backend is running.`);
  }
}

const apiClient = {
  get: <T>(path: string, headers?: Record<string, string>) => request<T>(path, { method: 'GET', headers }),
  post: <T>(path: string, body: unknown) => request<T>(path, { method: 'POST', body }),
  put: <T>(path: string, body: unknown) => request<T>(path, { method: 'PUT', body }),
  delete: <T>(path: string) => request<T>(path, { method: 'DELETE' }),
};

export default apiClient;