import axios from 'axios';

// Get the base URL from env variables, fallback to localhost for development
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5038/api';

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  // In development, handle self-signed certificates for local HTTPS
  // In a real app we'd configure this more securely
});

// Interceptor to attach auth token if available
apiClient.interceptors.request.use((config) => {
  if (typeof window !== 'undefined') {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

// Types based on our backend DTOs
export interface TermDto {
  id: string;
  nameEn: string;
  nameAr: string;
  nameFr?: string;
  category: string;
  definitionEn: string;
  definitionAr: string;
  exampleEn?: string;
  exampleAr?: string;
  isPublished: boolean;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}

export interface BookDto {
  id: string;
  title: string;
  authors: string[];
  year?: number;
  isbn?: string;
  language: string;
  category: string;
  coverUrl?: string;
  externalLink?: string;
  isIndexedForSearch: boolean;
  isPublished: boolean;
}

export interface AcademicEntryDto {
  id: string;
  title: string;
  author: string;
  university: string;
  year: number;
  supervisor?: string;
  abstract: string;
  documentUrl?: string;
  type: string;
  status: string;
  tags: string[];
}

export interface ExplanationResponse {
  explanation: string;
  language: string;
  style: string;
}

export interface SearchResultDto {
  type: string;
  id: string;
  title: string;
  snippet?: string;
  score: number;
  metadata: Record<string, any>;
}

export interface PaginatedResponse<T> {
  data: T[];
  meta: {
    page: number;
    size: number;
    total: number;
  };
}

// API Services
export const TermsService = {
  getTerms: (page = 1, size = 20) => 
    apiClient.get<PaginatedResponse<TermDto>>('/terms', { params: { page, size } }),
    
  getTerm: (id: string) => 
    apiClient.get<TermDto>(`/terms/${id}`),

  explainTerm: (id: string, language?: string, style?: string) => 
    apiClient.get<ExplanationResponse>(`/terms/${id}/explain`, { params: { language, style } })
};

export const BooksService = {
  getBooks: (page = 1, size = 20) => 
    apiClient.get<PaginatedResponse<BookDto>>('/books', { params: { page, size } }),
    
  uploadBook: (id: string, file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return apiClient.post(`/books/${id}/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
  }
};

export const AcademicService = {
  getEntries: (page = 1, size = 20) => 
    apiClient.get<PaginatedResponse<AcademicEntryDto>>('/academic', { params: { page, size } }),
};

export const SearchService = {
  search: (query: string, type?: string, page = 1, size = 20) => 
    apiClient.get<PaginatedResponse<SearchResultDto>>('/search', { params: { q: query, type, page, size } }),
};
