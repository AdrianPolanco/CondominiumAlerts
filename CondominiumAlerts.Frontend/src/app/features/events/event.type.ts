export type CondominiumEvent = {
    id: string;
    title: string;
    start: Date;
    end?: Date;
    allDay?: boolean;
    description?: string;
    color?: string;
}