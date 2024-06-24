import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Publication } from '../../shared/models/publication';
import { UserService } from "../../_services/user.service";
import { TokenStorageService } from '../../_services/token-storage.service';
import { Comments } from '../../shared/models/get-comment';
import { Author } from '../../shared/models/author';

@Component({
  selector: 'app-publication',
  templateUrl: './publication.component.html',
  styleUrls: ['./publication.component.css']
})
export class PublicationComponent implements OnInit {
  publication: Publication | null = null;
  author: Author | null = null;
  comments: Array<Comments> = [];
  isLoggedIn = false;
  login: string = '';
  newCommentText: string = '';
  loggedInUserId: string = '';
  newRating: number | null = null;
  ratingError: string = '';
  hasReviewed = false;  // To track if the user has already reviewed
  isSubmittingReview = false;  // To track if a review is being submitted
  userReviewId: number | null = null;  // To store the user's review ID

  constructor(
    private tokenStorageService: TokenStorageService,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }

  activeTab: string = 'book';

  showBookInfo() {
    this.activeTab = 'book';
  }

  showAuthorInfo() {
    this.activeTab = 'author';
  }

  showReviewsInfo() {
    this.activeTab = 'reviews';
  }

  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    
    const user = this.tokenStorageService.getUser();
    if (user) {
      this.loggedInUserId = user.userId || null;
      this.login = user.userId || null;
    }
  
    const publicationId = this.route.snapshot.params['publicationId'];
  
    this.userService.getPublicationById(publicationId).subscribe(publication => {
      this.publication = publication;
    });
  
    this.userService.getComments(publicationId).subscribe(comments => {
      this.comments = comments;
    });
  
    if (this.isLoggedIn && this.loggedInUserId) {
      this.userService.getOwnRewiew(publicationId, this.loggedInUserId).subscribe(review => {
        if (review) {
          this.hasReviewed = true;
          this.userReviewId = review.id;  
        }
      });
    }
  }
  
  onSubmitComment(): void {
    if (this.newCommentText.trim() && this.publication) {
      this.userService.createComment(this.publication.publicationId, this.newCommentText, this.login).subscribe(() => {
        this.userService.getComments(this.publication!.publicationId).subscribe(comments => {
          this.comments = comments;
          this.newCommentText = '';
        });
      });
    }
  }

  onDeleteComment(commentId: number): void {
    this.userService.deleteComment(commentId).subscribe(() => {
      this.comments = this.comments.filter(comment => comment.commentId !== commentId);
    });
  }

  onSubmitReview(): void {
    if (this.isSubmittingReview) {
      return; // Prevent multiple submissions
    }
  
    if (this.newRating !== null && this.newRating >= 0 && this.newRating <= 100 && this.publication && !this.hasReviewed) {
      this.isSubmittingReview = true;  // Set flag to indicate submission in progress
      this.userService.addRewiew(this.publication.publicationId, this.newRating, this.loggedInUserId).subscribe(() => {
        this.userService.getPublicationById(this.publication!.publicationId).subscribe(publication => {
          this.publication = publication;
        });
        this.userService.getOwnRewiew(this.publication!.publicationId, this.loggedInUserId).subscribe(review => {
          if (review) {
            this.hasReviewed = true;
            this.userReviewId = review.id;  
          }
          this.isSubmittingReview = false; 
          this.ratingError = '';
        });
      }, () => {
        this.isSubmittingReview = false; 
      });
    } else {
      this.ratingError = 'Please enter a rating between 0 and 100.';
    }
  }
  onDeleteReview(): void {
    if (this.userReviewId) {
      this.userService.deleteRewiew(this.userReviewId).subscribe(() => {
        this.hasReviewed = false;
        this.userReviewId = null;
        this.userService.getPublicationById(this.publication!.publicationId).subscribe(publication => {
          this.publication = publication;
        });
      });
    }
  }
}
