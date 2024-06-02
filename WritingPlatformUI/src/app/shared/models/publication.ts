import { Author } from "./author";

export interface Publication {
  publicationId: number;
  publicationName: string;
  genreName: string;
  authorName: string;
  rating: number;
  datePublication: Date;
  titleKey: string;
  
  bookDescription: string;
  author: Author;
}
