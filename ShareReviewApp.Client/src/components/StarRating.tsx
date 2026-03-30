interface Props {
  rating: number;
  max?: number;
}

export default function StarRating({ rating, max = 10 }: Props) {
  return (
    <span className="flex items-center gap-1 text-sm">
      <span className="text-amber-400 font-semibold">{rating}</span>
      <span className="text-gray-400">/ {max}</span>
      <span className="text-amber-400">{'★'.repeat(Math.round((rating / max) * 5))}{'☆'.repeat(5 - Math.round((rating / max) * 5))}</span>
    </span>
  );
}
