#include <bits/stdc++.h>
#define debuglog(s) std::cout << "[debug]: " << s << std::endl
using namespace std;

class linked_circular_vector
{
	class node
	{
	public:
		int val;
		std::shared_ptr<node> prev;
		std::shared_ptr<node> next;
	};

	std::shared_ptr<node> head, tail; // tail->head

public:
	linked_circular_vector()
		: head(nullptr), tail(nullptr)
	{
	}

	void add_last(int x)
	{
		std::shared_ptr<node> curr = std::make_shared<node>();
		curr->val = x;
		curr->next = tail;
		
		if (tail != nullptr)
		{
			tail->prev = curr;
		}
		else
		{
			head = curr;
		}
		tail = curr;
	}
	
	bool remove_first()
	{
		if (head == nullptr) return false;
		head = head->prev;
		if (head != nullptr)
		{
			head->next = nullptr;
		}
		else
		{
			tail = nullptr; // 一起变 nullptr
		}

		return true;
	}

	int peek_first()
	{
		if (head == nullptr) throw std::runtime_error("size is 0");
		return head->val;
	}
};

class circular_vector
{
	unique_ptr<int[]> arr;
	int capacity;

	int tail, head; // index [tail, head)

public:
	circular_vector()
	{
		capacity = 5;
		arr = std::make_unique<int[]>(capacity);
	}

	void add_last(int x)
	{
		//if (get_mapped_index(tail - 1) == head) throw std::runtime_error("is full"); // maxsize == capacity - 1
		if (get_mapped_index(tail - 1) == head)
		{
			resize_array(2 * capacity);
		}

		tail = get_mapped_index(tail - 1);
		arr[tail] = x;
	}

	bool remove_first()
	{
		if (head == tail) return false;
		head = get_mapped_index(head - 1);
		return true;
	}

	int peek_first()
	{
		if (head == tail) throw std::runtime_error("size is 0");
		return arr[get_mapped_index(head - 1)];
	}

	class iterator
	{
		circular_vector& origin;
		int index;

	public:
		iterator(circular_vector& origin, int index) : origin(origin), index(index)
		{
		}

		bool operator!=(const iterator& other) const
		{
			return other.index != index;
		}

		bool operator==(const iterator& other) const
		{
			return other.index == index;
		}

		iterator& operator++()
		{
			index = origin.get_mapped_index(index - 1);
			return *this;
		}

		iterator& operator++(int x) = delete;

		int operator*()
		{
			return origin.arr[index];
		}
	};

	iterator begin()
	{
		return iterator(*this, get_mapped_index(head - 1));
	}

	iterator end()
	{
		return iterator(*this, get_mapped_index(tail - 1));
	}

private:
	int get_mapped_index(int x) const
	{
		int n = capacity;
		return x < 0 ? n - ((-x) % n) : x % n;
	}

	void resize_array(int new_capacity)
	{
		if (new_capacity < capacity) throw std::runtime_error("invalid operation");

		auto new_arr = std::make_unique<int[]>(new_capacity);

		int cnt = 0;
		for (int i = tail; i != head; i = get_mapped_index(i + 1))
		{
			new_arr[cnt] = arr[i];
			cnt++;
		}

		arr = std::move(new_arr);

		tail = 0;
		head = cnt;
		capacity = new_capacity;
	}
};

class m_queue
{
public:
	linked_circular_vector container;
	
public:
	void push(int x)
	{
		container.add_last(x);
	}

	bool pop()
	{
		return container.remove_first();
	}

	int front()
	{
		return container.peek_first();
	}
};

int main1()
{
	circular_vector cv;
	cv.add_last(1);
	cv.add_last(2);
	cv.add_last(3);
	cv.add_last(4);

	cout << cv.peek_first() << endl;
	cv.remove_first();

	cv.add_last(5);
	cv.add_last(6);

	for (auto it = cv.begin(); it != cv.end(); ++it)
	{
		cout << *it << ' ';
	}

	cout << endl;
	return 0;
}

int main()
{
	m_queue mq;

	auto pop_and_log = [&]() {
			try
			{
				cout << "mq: " << mq.front() << endl;
				mq.pop();

			}
			catch (const std::exception& e)
			{
				cout << "ex: " << e.what() << endl;
			}
		};

	mq.push(1);
	mq.push(2);
	mq.push(3);

	pop_and_log();
	pop_and_log();
	pop_and_log();

	try
	{
		cout << "mq: " << mq.front() << endl;
	}
	catch (const std::exception& e)
	{
		cout << e.what() << endl;
	}

	try
	{
		mq.pop();
	}
	catch (const std::exception& e)
	{
		cout << e.what() << endl;
	}

	mq.push(1);
	mq.push(2);
	mq.push(3);
	mq.push(4);

	try
	{
		mq.push(5);
		cout << "push success" << endl;
	}
	catch (const std::exception& e)
	{
		cout << e.what() << endl;
	}

	pop_and_log();
	pop_and_log();
	pop_and_log();
	pop_and_log();
	pop_and_log();
	pop_and_log();

	for (int i = 0; i < 20; i++)
	{
		mq.push(i);
	}

	for (int i = 0; i < 5; i++)
	{
		pop_and_log();
	}

	for (int i = 30; i < 40; i++)
	{
		mq.push(i);
	}

	for (int i = 0; i < 7; i++)
	{
		pop_and_log();
	}

	mq.push(70);
	pop_and_log();

	for (int i = 0; i < 50; i++)
	{
		pop_and_log();
	}
	
	return 0;
}
