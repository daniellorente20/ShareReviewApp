export interface Product {
  id: string;
  name: string;
  description: string;
  category: string;
  createdAt: string;
}

export interface Review {
  id: string;
  userId: string;
  productId: string;
  comment: string;
  rating: number;
  createdAt: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  category: string;
}

export interface CreateReviewRequest {
  userId: string;
  productId: string;
  comment: string;
  rating: number;
}

// Seeded users — no user endpoints, hardcoded from AppDbContext seed
export const SEEDED_USERS = [
  { id: '11111111-1111-1111-1111-111111111111', name: 'Alice Smith' },
  { id: '22222222-2222-2222-2222-222222222222', name: 'Bob Jones' },
] as const;
