import { SummaryStatus } from "./summaryStatus.enum";

export type GetSummaryStatusResponse = {
    condominiumId: string;
    status: SummaryStatus;
}