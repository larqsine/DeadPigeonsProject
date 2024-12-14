import { atom } from 'jotai';

export type User = {
    id: string;
    userName: string;
    fullName: string;
    email: string;
    phoneNumber: string;
    balance: number;
    annualFeePaid: boolean;
    createdAt: string;
};

export interface Transaction {
    id: string;
    amount: number;
    status: number;
    transactionType: string;
    createdAt?: string; // Optional
}

interface Board {
    id: string;
    numbers: string;
    fieldsCount: number;
    cost: number;
    createdAt: string;
    isWinning: boolean;
}

// Atoms for AdminPage
export const selectedWinningNumbersAtom = atom<number[]>([]);
export const usersAtom = atom<User[]>([]);
export const selectedUserAtom = atom<User | null>(null);
export const isModalOpenAtom = atom(false);
export const isCreateUserModalOpenAtom = atom(false);
export const isEditUserModalOpenAtom = atom(false);
export const editUserAtom = atom<User | null>(null);
export const authAtom = atom<{ token: string | null }>({ token: null });

export const newUserAtom = atom({
    userName: '',
    fullName: '',
    email: '',
    phoneNumber: '',
    password: '',
    role: '',
});

// Atoms for Play page
export const selectedBoxesAtom = atom<number[]>([]);
export const playerIdAtom = atom<string>('');
export const gameIdAtom = atom<string>('');
export const messageAtom = atom<string>('');
export const errorAtom = atom<string>('');

// Atoms for LoginPage
export const loginFormAtom = atom({ email: '', password: '' });
export const userAtom = atom<{ userName: string; roles: string[]; token: string[]; passwordChangeRequired: boolean } | null>(null);
export const isLoggedInAtom = atom<boolean>(false);

// Atoms for transactions page
export const transactionsAtom = atom<Transaction[]>([]);
export const loadingAtom = atom(true);

export const boardHistoryAtom = atom<Board[]>([]);
