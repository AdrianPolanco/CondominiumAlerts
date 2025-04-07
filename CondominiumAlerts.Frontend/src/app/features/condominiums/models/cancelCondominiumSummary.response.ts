export type CancelCondominiumSummaryResponse = {
    cancelled: boolean;
    message: string;
    condominiumId: string;
    jobId: string;
    requestedByUserId: string;
};