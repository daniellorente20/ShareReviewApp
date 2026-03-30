import type { CreateReviewRequest, Review } from '../types';

export async function getReviewsByProduct(productId: string): Promise<Review[]> {
  const res = await fetch(`/api/products/${productId}/reviews`);
  if (!res.ok) throw new Error('Failed to fetch reviews');
  return res.json();
}

export async function createReview(data: CreateReviewRequest): Promise<Review> {
  const res = await fetch('/api/reviews', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });

  if (res.status === 409) throw new Error('You have already reviewed this product');
  if (res.status === 404) {
    const body = await res.json();
    throw new Error(body.message ?? 'User or product not found');
  }
  if (!res.ok) throw new Error('Failed to submit review');
  return res.json();
}
