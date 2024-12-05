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

// Atoms for AdminPage
export const selectedWinningNumbersAtom = atom<number[]>([]);
export const usersAtom = atom<User[]>([]);
export const selectedUserAtom = atom<User | null>(null);
export const isModalOpenAtom = atom(false);
export const isCreateUserModalOpenAtom = atom(false);
export const isEditUserModalOpenAtom = atom(false);
export const editUserAtom = atom<User | null>(null);

export const newUserAtom = atom({
    userName: '',
    fullName: '',
    email: '',
    phoneNumber: '',
    password: '',
    role: '',
});

// Atoms for BoxGrid component
export const selectedBoxesAtom = atom<number[]>([]);
export const playerIdAtom = atom<string>('');
export const gameIdAtom = atom<string>('');
export const messageAtom = atom<string>('');
export const errorAtom = atom<string>('');

// Atoms for LoginPage component
export const loginFormAtom = atom({ email: '', password: '' }); // To store the email and password input
export const userAtom = atom<{ userName: string; roles: string[]; token: string[]; } | null>(null); // To store logged-in user details
export const isLoggedInAtom = atom<boolean>(false);

// Atoms for NavBar component
export const isDropdownOpenAtom = atom(false);
export const isAddBalanceOpenAtom = atom(false); 
export const transactionIdAtom = atom(''); 
export const balanceAtom = atom(0); 
export const usernameAtom = atom(''); 
export const isAdminAtom = atom(false); 